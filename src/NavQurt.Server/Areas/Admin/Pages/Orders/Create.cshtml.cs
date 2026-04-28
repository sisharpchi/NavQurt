using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Orders;

public class CreateModel(
    IOrderService orderService,
    IProductService productService,
    IProductCategoryService categoryService,
    IPaymentMethodService paymentMethodService,
    IWorkerService workerService,
    IWarehouseService warehouseService) : PageModel
{
    private const int OrderRows = 24;

    [BindProperty]
    public OrderInput Input { get; set; } = CreateEmptyInput();

    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();
    public IReadOnlyCollection<PaymentMethodDto> PaymentMethods { get; private set; } = Array.Empty<PaymentMethodDto>();
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        Input.PaymentMethodId = PaymentMethods.FirstOrDefault()?.Id ?? 0;
        Input.WorkerId = Workers.FirstOrDefault()?.Id ?? 0;
        Input.WarehouseId = Warehouses.FirstOrDefault(x => x.IsMain)?.Id ?? Warehouses.FirstOrDefault()?.Id ?? 0;
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var items = Input.Items
            .Where(x => x.ProductId > 0 && x.Quantity > 0)
            .Select(x => new CreateOrderItemRequest(x.ProductId, x.Quantity))
            .ToList();

        var result = await orderService.CreateAsync(
            new CreateOrderRequest(
                Input.WarehouseId,
                Input.WorkerId,
                Input.PaymentMethodId,
                Input.CustomerPhoneNumber,
                Input.CustomerFullName,
                Input.CustomerLocation,
                Input.Comment,
                items),
            cancellationToken);

        Message = result.Success ? $"Order yaratildi: {result.Value.OrderNumber}" : result.Error;
        return RedirectToPage();
    }

    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Products = ((await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>())
            .Where(x => x.IsActive)
            .ToList();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
        PaymentMethods = ((await paymentMethodService.GetListAsync(cancellationToken)).Value ?? Array.Empty<PaymentMethodDto>())
            .Where(x => x.IsActive)
            .ToList();
        Workers = ((await workerService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>())
            .Where(x => x.IsActive)
            .ToList();
        Warehouses = ((await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>())
            .Where(x => x.IsActive)
            .ToList();
    }

    private static OrderInput CreateEmptyInput() => new()
    {
        Items = Enumerable.Range(0, OrderRows).Select(_ => new OrderItemInput()).ToList()
    };

    public class OrderInput
    {
        public int WarehouseId { get; set; }
        public int WorkerId { get; set; }
        public int PaymentMethodId { get; set; }
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public string CustomerFullName { get; set; } = string.Empty;
        public string? CustomerLocation { get; set; }
        public string? Comment { get; set; }
        public List<OrderItemInput> Items { get; set; } = new();
    }

    public class OrderItemInput
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
