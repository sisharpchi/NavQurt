using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Orders;

public class IndexModel(IOrderService orderService, IWorkerService workerService, IWarehouseService warehouseService) : PageModel
{
    public IReadOnlyCollection<OrderDto> Orders { get; private set; } = Array.Empty<OrderDto>();
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    public int ItemsCount { get; private set; }
    public decimal TotalAmount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? WorkerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? WarehouseId { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await orderService.GetListAsync(new OrderListRequest(FromDate, ToDate, Search, WorkerId, WarehouseId), cancellationToken);
        Orders = result.Value?.Items ?? Array.Empty<OrderDto>();
        ItemsCount = result.Value?.ItemsCount ?? Orders.Count;
        TotalAmount = result.Value?.TotalAmount ?? Orders.Sum(x => x.TotalAmount);
        Workers = (await workerService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>();
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
    }

    public string GetWorkerName(OrderDto item) =>
        !string.IsNullOrWhiteSpace(item.WorkerFullName) ? item.WorkerFullName : Workers.FirstOrDefault(x => x.Id == item.WorkerId)?.FullName ?? "-";

    public string GetWarehouseTitle(OrderDto item) =>
        !string.IsNullOrWhiteSpace(item.WarehouseTitle) ? item.WarehouseTitle : Warehouses.FirstOrDefault(x => x.Id == item.WarehouseId)?.Title ?? "-";
}
