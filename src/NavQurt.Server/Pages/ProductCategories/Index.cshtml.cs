using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.ProductCategories;

public class IndexModel(IProductCategoryService service) : PageModel
{
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Categories = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
    }

    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";
}
