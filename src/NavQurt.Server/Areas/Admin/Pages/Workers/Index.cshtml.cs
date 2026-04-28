using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Workers;

public class IndexModel(IWorkerService service) : PageModel
{
    [BindProperty]
    public WorkerInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<WorkerDto> Workers { get; private set; } = Array.Empty<WorkerDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Workers.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new WorkerInput { Id = item.Id, FullName = item.FullName, PhoneNumber = item.PhoneNumber, IsActive = item.IsActive };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new WorkerRequest(Input.FullName, Input.PhoneNumber, Input.IsActive);
        var result = Input.Id > 0
            ? await service.UpdateAsync(Input.Id, request, cancellationToken)
            : await service.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Ishchi saqlandi." : result.Error;
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Workers = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<WorkerDto>();
    }

    public class WorkerInput
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
