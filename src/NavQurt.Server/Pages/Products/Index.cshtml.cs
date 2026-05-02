using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Products;

public class IndexModel(IProductService productService, IProductCategoryService categoryService) : PageModel
{
    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ProductType { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool OnlyWithoutRecipe { get; set; }

    [TempData] public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await productService.GetListAsync(
            new ProductListRequest(Search, CategoryId, ParseBool(Status), ParseBool(ProductType), OnlyWithoutRecipe),
            cancellationToken);

        Products = result.Value?.Items ?? Array.Empty<ProductDto>();
        ItemsCount = result.Value?.ItemsCount ?? Products.Count;
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
    }

    public string GetCategoryTitle(ProductDto item) =>
        item.ProductCategoryTitle ?? (item.ProductCategoryId.HasValue ? Categories.FirstOrDefault(x => x.Id == item.ProductCategoryId.Value)?.Title ?? "-" : "-");

    private static bool? ParseBool(string? value) =>
        bool.TryParse(value, out var parsed) ? parsed : null;
}
