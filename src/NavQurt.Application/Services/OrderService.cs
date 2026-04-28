using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Enumerations;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class OrderService(
    IMainRepository repository,
    RecipeCalculator calculator,
    StockMutationService stockMutation) : BusinessServiceBase, IOrderService
{
    public async Task<ResponseResult<IReadOnlyCollection<OrderDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await OrderQuery()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<OrderDto>>.CreateSuccess(items.Select(x => x.ToDto()).ToList());
    }

    public async Task<ResponseResult<OrderDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await OrderQuery().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity == null ? NotFound<OrderDto>("Order") : ResponseResult<OrderDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<OrderDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToArray();
        var products = await repository.Query<Product>()
            .Where(x => productIds.Contains(x.Id) && !x.IsDeleted && x.IsActive)
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var writeOff = new Dictionary<int, decimal>();
        foreach (var item in request.Items)
        {
            var calculated = await calculator.CalculateForProductAsync(item.ProductId, item.Quantity, cancellationToken);
            if (!calculated.Success)
            {
                return ResponseResult<OrderDto>.CreateError(calculated.Error!, calculated.ErrorCode);
            }

            foreach (var (ingredientId, quantity) in calculated.Value)
            {
                writeOff[ingredientId] = writeOff.TryGetValue(ingredientId, out var current) ? current + quantity : quantity;
            }
        }

        await using var transaction = await repository.Database.BeginTransactionAsync(cancellationToken);

        var customer = await repository.Query<Customer>()
            .FirstOrDefaultAsync(x => x.PhoneNumber == request.CustomerPhoneNumber && !x.IsDeleted, cancellationToken);

        if (customer == null)
        {
            customer = new Customer
            {
                PhoneNumber = request.CustomerPhoneNumber.Trim(),
                FullName = request.CustomerFullName.Trim(),
                Location = request.CustomerLocation
            };
            await repository.AddAsync(customer);
        }
        else
        {
            customer.FullName = request.CustomerFullName.Trim();
            customer.Location = request.CustomerLocation;
        }

        var now = DateTime.UtcNow;
        var order = new Order
        {
            OrderNumber = $"NQ-{now:yyyyMMddHHmmssfff}",
            Customer = customer,
            WorkerId = request.WorkerId,
            WarehouseId = request.WarehouseId,
            CreatedAt = now,
            PaidAt = now,
            CustomerPhoneNumber = request.CustomerPhoneNumber.Trim(),
            CustomerFullName = request.CustomerFullName.Trim(),
            CustomerLocation = request.CustomerLocation,
            Comment = request.Comment,
            Status = OrderStatus.Paid
        };

        foreach (var item in request.Items)
        {
            var product = products[item.ProductId];
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductTitle = product.Title,
                Quantity = item.Quantity,
                Price = product.Price,
                IsCombo = product.IsCombo
            });
        }

        order.TotalAmount = order.Items.Sum(x => x.Quantity * x.Price);
        order.Payments.Add(new OrderPayment { PaymentMethodId = request.PaymentMethodId, Amount = order.TotalAmount });

        await repository.AddAsync(order);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        foreach (var (ingredientId, quantity) in writeOff)
        {
            await stockMutation.ApplyAsync(request.WarehouseId, ingredientId, -quantity, StockMovementType.Order, null, order.Id, $"Order {order.OrderNumber}", cancellationToken);
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        order = await OrderQuery().FirstAsync(x => x.Id == order.Id, cancellationToken);
        return ResponseResult<OrderDto>.CreateSuccess(order.ToDto());
    }

    private IQueryable<Order> OrderQuery() =>
        repository.Query<Order>()
            .Include(x => x.Items)
            .Include(x => x.Payments)
            .ThenInclude(x => x.PaymentMethod);

    private async Task<ResponseResult> ValidateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
        {
            return ResponseResult.CreateError("Order itemlar bo'sh bo'lmasligi kerak.");
        }

        if (!HasText(request.CustomerPhoneNumber) || !HasText(request.CustomerFullName))
        {
            return ResponseResult.CreateError("Customer phone va FIO majburiy.");
        }

        if (request.Items.Any(x => x.Quantity <= 0))
        {
            return ResponseResult.CreateError("Order item quantity 0 dan katta bo'lishi kerak.");
        }

        if (!await repository.Query<Warehouse>().AnyAsync(x => x.Id == request.WarehouseId && !x.IsDeleted && x.IsActive, cancellationToken))
        {
            return ResponseResult.CreateError("Warehouse topilmadi.", WebErrorConstant.WarehouseNotExists);
        }

        if (!await repository.Query<Worker>().AnyAsync(x => x.Id == request.WorkerId && !x.IsDeleted && x.IsActive, cancellationToken))
        {
            return ResponseResult.CreateError("Worker topilmadi.", WebErrorConstant.NoAccessibleWorkerForCompany);
        }

        if (!await repository.Query<PaymentMethod>().AnyAsync(x => x.Id == request.PaymentMethodId && !x.IsDeleted && x.IsActive, cancellationToken))
        {
            return ResponseResult.CreateError("Payment method topilmadi.", WebErrorConstant.PaymentMethodNotExists);
        }

        var productIds = request.Items.Select(x => x.ProductId).Distinct().ToArray();
        var productCount = await repository.Query<Product>().CountAsync(x => productIds.Contains(x.Id) && !x.IsDeleted && x.IsActive, cancellationToken);
        if (productCount != productIds.Length)
        {
            return ResponseResult.CreateError("Product topilmadi.", WebErrorConstant.ProductNotExists);
        }

        return ResponseResult.CreateSuccess();
    }
}
