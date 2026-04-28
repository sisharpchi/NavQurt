using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.PaymentMethods;

public class IndexModel(IPaymentMethodService service) : PageModel
{
    [BindProperty]
    public PaymentMethodInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<PaymentMethodDto> PaymentMethods { get; private set; } = Array.Empty<PaymentMethodDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = PaymentMethods.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new PaymentMethodInput { Id = item.Id, Title = item.Title, IsActive = item.IsActive };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new PaymentMethodRequest(Input.Title, Input.IsActive);
        var result = Input.Id > 0
            ? await service.UpdateAsync(Input.Id, request, cancellationToken)
            : await service.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Payment method saqlandi." : result.Error;
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        PaymentMethods = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<PaymentMethodDto>();
    }

    public class PaymentMethodInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
