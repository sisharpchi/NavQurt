using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Warehouses;

public class IndexModel(IWarehouseService service) : PageModel
{
    [BindProperty]
    public WarehouseInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<WarehouseDto> Warehouses { get; private set; } = Array.Empty<WarehouseDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Warehouses.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new WarehouseInput { Id = item.Id, Title = item.Title, IsActive = item.IsActive };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new WarehouseRequest(Input.Title, Input.IsActive);
        var result = Input.Id > 0
            ? await service.UpdateAsync(Input.Id, request, cancellationToken)
            : await service.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Sklad saqlandi." : result.Error;
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Warehouses = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<WarehouseDto>();
    }

    public class WarehouseInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
