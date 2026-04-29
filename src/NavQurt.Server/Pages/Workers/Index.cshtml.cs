using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Workers;

public class IndexModel(IWorkerService service) : PageModel
{
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Workers = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>();
    }
}
