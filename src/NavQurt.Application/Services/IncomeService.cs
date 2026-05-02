using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Enumerations;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class IncomeService(IMainRepository repository, StockMutationService stockMutation) : BusinessServiceBase, IIncomeService
{
    public async Task<ResponseResult<IReadOnlyCollection<IncomeDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new IncomeListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<IncomeDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<IncomeDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<IncomeListResponse>> GetListAsync(IncomeListRequest request, CancellationToken cancellationToken = default)
    {
        var items = await repository.Query<Income>()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .AsQueryable()
            .ApplyFilter(request)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var response = new IncomeListResponse(
            items.Select(x => x.ToDto()).ToList(),
            items.Count,
            items.Sum(x => x.TotalAmount));

        return ResponseResult<IncomeListResponse>.CreateSuccess(response);
    }

    public async Task<ResponseResult<IncomeDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.Query<Income>()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? NotFound<IncomeDto>("Income") : ResponseResult<IncomeDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<IncomeDto>> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest<IncomeDto>("Income itemlar bo'sh bo'lmasligi kerak.");
        }

        if (!await repository.Query<Warehouse>().AnyAsync(x => x.Id == request.WarehouseId && !x.IsDeleted && x.IsActive, cancellationToken))
        {
            return ResponseResult<IncomeDto>.CreateError("Warehouse topilmadi.", WebErrorConstant.WarehouseNotExists);
        }

        var normalizedItems = NormalizeItems(request.Items);
        if (normalizedItems.Count == 0)
        {
            return BadRequest<IncomeDto>("Income itemlar bo'sh bo'lmasligi kerak.");
        }

        var ingredientIds = normalizedItems.Select(x => x.IngredientId).Distinct().ToArray();
        var foundIngredients = await repository.Query<Ingredient>().CountAsync(x => ingredientIds.Contains(x.Id) && !x.IsDeleted, cancellationToken);
        if (foundIngredients != ingredientIds.Length)
        {
            return ResponseResult<IncomeDto>.CreateError("Ingredient topilmadi.", WebErrorConstant.IngredientNotExists);
        }

        if (normalizedItems.Any(x => x.Quantity <= 0 || x.Price < 0))
        {
            return BadRequest<IncomeDto>("Income quantity 0 dan katta, price esa 0 yoki katta bo'lishi kerak.");
        }

        if (request.IncomedAt.HasValue && request.IncomedAt.Value > DateTime.Now)
        {
            return BadRequest<IncomeDto>("Prixod sanasi kelajakda bo'lishi mumkin emas.");
        }

        await using var transaction = await repository.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var incomedAt = request.IncomedAt?.ToUniversalTime() ?? now;
        var income = new Income
        {
            WarehouseId = request.WarehouseId,
            CreatedAt = incomedAt,
            AcceptedAt = now,
            Comment = request.Comment,
            TotalAmount = normalizedItems.Sum(x => x.Quantity * x.Price)
        };

        foreach (var item in normalizedItems)
        {
            income.Items.Add(new IncomeItem
            {
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        await repository.AddAsync(income);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        foreach (var item in income.Items)
        {
            await stockMutation.ApplyAsync(request.WarehouseId, item.IngredientId, item.Quantity, StockMovementType.Income, income.Id, null, "Income", cancellationToken);
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        income = await repository.Query<Income>()
            .Include(x => x.Warehouse)
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .FirstAsync(x => x.Id == income.Id, cancellationToken);

        return ResponseResult<IncomeDto>.CreateSuccess(income.ToDto());
    }

    private static List<CreateIncomeItemRequest> NormalizeItems(IEnumerable<CreateIncomeItemRequest> items)
    {
        return items
            .Where(x => x.IngredientId > 0 && x.Quantity > 0)
            .GroupBy(x => x.IngredientId)
            .Select(group =>
            {
                var quantity = group.Sum(x => x.Quantity);
                var amount = group.Sum(x => x.Quantity * x.Price);
                var price = quantity == 0 ? 0 : amount / quantity;
                return new CreateIncomeItemRequest(group.Key, quantity, price);
            })
            .ToList();
    }
}

internal static class IncomeQueryableExtensions
{
    public static IQueryable<Income> ApplyFilter(this IQueryable<Income> query, IncomeListRequest request)
    {
        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= request.FromDate.Value.Date.ToUniversalTime());
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.CreatedAt < request.ToDate.Value.Date.AddDays(1).ToUniversalTime());
        }

        if (request.WarehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == request.WarehouseId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                (x.Comment != null && x.Comment.ToLower().Contains(search)) ||
                x.Items.Any(item => item.Ingredient.Title.ToLower().Contains(search)));
        }

        return query;
    }
}
