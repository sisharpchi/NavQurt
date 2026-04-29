using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.PaymentMethods;

public class DeleteModel(IPaymentMethodService service) : PageModel
{
    public PaymentMethodDto? Item { get; private set; }

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Item = result.Value;
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        Message = result.Success ? "Yozuv o'chirildi." : result.Error;
        return RedirectToPage("./Index");
    }
}
