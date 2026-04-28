using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Customers;

public class IndexModel(ICustomerService service) : PageModel
{
    public IReadOnlyCollection<CustomerDto> Customers { get; private set; } = Array.Empty<CustomerDto>();

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Customers = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<CustomerDto>();
    }
}
