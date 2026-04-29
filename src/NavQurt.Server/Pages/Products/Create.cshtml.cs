using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Products;

public class CreateModel(IProductService productService, IProductCategoryService categoryService) : PageModel
{
    private const int ComboRows = 5;
    [BindProperty] public ProductInput Input { get; set; } = CreateEmptyInput();
    public int Id => 0;
    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task OnGet(CancellationToken cancellationToken) => await LoadAsync(cancellationToken);
    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var result = await productService.CreateAsync(ToRequest(), cancellationToken);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error); await LoadAsync(cancellationToken); return Page(); }
        Message = "Product yaratildi.";
        return RedirectToPage("./Index");
    }
    private ProductRequest ToRequest() => new(Input.Title, Input.Description, Input.Price, Input.SelfPrice, Input.IsActive, Input.IsCombo, Input.UseOwnRecipeForCombo, Input.ProductCategoryId, Input.ComboItems.Where(x => x.ProductId > 0 && x.Quantity > 0).Select(x => new ProductComboItemRequest(x.ProductId, x.Quantity)).ToList());
    private async Task LoadAsync(CancellationToken cancellationToken) { Products = (await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>(); Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>(); }
    private static ProductInput CreateEmptyInput() => new() { IsActive = true, ComboItems = Enumerable.Range(0, ComboRows).Select(_ => new ComboItemInput { Quantity = 1 }).ToList() };
    public class ProductInput { public string Title { get; set; } = string.Empty; public string? Description { get; set; } public decimal Price { get; set; } public decimal SelfPrice { get; set; } public bool IsActive { get; set; } public bool IsCombo { get; set; } public bool UseOwnRecipeForCombo { get; set; } public int? ProductCategoryId { get; set; } public List<ComboItemInput> ComboItems { get; set; } = new(); }
    public class ComboItemInput { public int ProductId { get; set; } public decimal Quantity { get; set; } }
}