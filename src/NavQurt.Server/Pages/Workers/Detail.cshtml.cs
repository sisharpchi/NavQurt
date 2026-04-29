using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Workers;

public class DetailModel(IWorkerService service) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Input = new InputModel { FullName = result.Value.FullName, PhoneNumber = result.Value.PhoneNumber, IsActive = result.Value.IsActive };
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, new WorkerRequest(Input.FullName, Input.PhoneNumber, Input.IsActive), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }
        Message = "Ishchi saqlandi.";
        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}