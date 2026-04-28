using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Orders;

public class IndexModel(IOrderService orderService, IWorkerService workerService) : PageModel
{
    public IReadOnlyCollection<OrderDto> Orders { get; private set; } = Array.Empty<OrderDto>();
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Orders = (await orderService.GetListAsync(cancellationToken)).Value ?? Array.Empty<OrderDto>();
        Workers = (await workerService.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>();
    }

    public string GetWorkerName(int id) => Workers.FirstOrDefault(x => x.Id == id)?.FullName ?? "-";
}
