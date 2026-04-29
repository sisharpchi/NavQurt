using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Warehouses;

public class IndexModel(IWarehouseService service) : PageModel
{
    public IReadOnlyCollection<WarehouseDto> Items { get; private set; } = Array.Empty<WarehouseDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Items = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
    }
}
