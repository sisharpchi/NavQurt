using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.PaymentMethods;

public class IndexModel(IPaymentMethodService service) : PageModel
{
    public IReadOnlyCollection<PaymentMethodDto> Items { get; private set; } = Array.Empty<PaymentMethodDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Items = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<PaymentMethodDto>();
    }
}
