using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Workers;

public class IndexModel(IWorkerService service) : PageModel
{
    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await service.GetListAsync(new WorkerListRequest(Search, ParseBool(Status)), cancellationToken);
        Workers = result.Value?.Items ?? Array.Empty<WorkerDto>();
        ItemsCount = result.Value?.ItemsCount ?? Workers.Count;
    }

    private static bool? ParseBool(string? value) =>
        bool.TryParse(value, out var parsed) ? parsed : null;
}
