using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Customers;

public class IndexModel(ICustomerService service) : PageModel
{
    public IReadOnlyCollection<CustomerDto> Customers { get; private set; } = Array.Empty<CustomerDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await service.GetListAsync(new CustomerListRequest(Search), cancellationToken);
        Customers = result.Value?.Items ?? Array.Empty<CustomerDto>();
        ItemsCount = result.Value?.ItemsCount ?? Customers.Count;
    }
}
