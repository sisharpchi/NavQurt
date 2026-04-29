using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Orders;

public class DetailModel(IOrderService orderService, IWorkerService workerService, IWarehouseService warehouseService) : PageModel
{
    public OrderDto? Order { get; private set; }
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();
    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();
    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        Workers = (await workerService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>();
        Warehouses = (await warehouseService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
        var result = await orderService.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Order = result.Value;
        return Page();
    }
    public string GetWorkerName(int id) => Workers.FirstOrDefault(x => x.Id == id)?.FullName ?? "-";
    public string GetWarehouseTitle(int id) => Warehouses.FirstOrDefault(x => x.Id == id)?.Title ?? "-";
}