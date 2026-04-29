using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Products;

public class IndexModel(IProductService productService, IProductCategoryService categoryService) : PageModel
{
    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken)
    {
        Products = (await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
    }
    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";
}
