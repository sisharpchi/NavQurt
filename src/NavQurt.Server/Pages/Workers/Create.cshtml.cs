using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Workers;

public class CreateModel(IWorkerService service) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new() { IsActive = true };

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(new WorkerRequest(Input.FullName, Input.PhoneNumber, Input.IsActive), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        Message = "Ishchi yaratildi.";
        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}