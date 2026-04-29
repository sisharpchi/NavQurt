using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.Products;

public class DetailModel(IProductService productService, IProductCategoryService categoryService) : PageModel
{
    private const int ComboRows = 5;
    [BindProperty] public ProductInput Input { get; set; } = new();
    public int Id { get; private set; }
    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();
    [TempData] public string? Message { get; set; }
    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        Id = id; await LoadAsync(cancellationToken);
        var result = await productService.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();
        Input = new ProductInput { Title = result.Value.Title, Description = result.Value.Description, Price = result.Value.Price, SelfPrice = result.Value.SelfPrice, IsActive = result.Value.IsActive, IsCombo = result.Value.IsCombo, UseOwnRecipeForCombo = result.Value.UseOwnRecipeForCombo, ProductCategoryId = result.Value.ProductCategoryId, ComboItems = Pad(result.Value.ComboItems.Select(x => new ComboItemInput { ProductId = x.ProductId, Quantity = x.Quantity }).ToList()) };
        return Page();
    }
    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        Id = id;
        var result = await productService.UpdateAsync(id, ToRequest(), cancellationToken);
        if (!result.Success) { ModelState.AddModelError(string.Empty, result.Error); await LoadAsync(cancellationToken); return Page(); }
        Message = "Product saqlandi.";
        return RedirectToPage("./Index");
    }
    private ProductRequest ToRequest() => new(Input.Title, Input.Description, Input.Price, Input.SelfPrice, Input.IsActive, Input.IsCombo, Input.UseOwnRecipeForCombo, Input.ProductCategoryId, Input.ComboItems.Where(x => x.ProductId > 0 && x.Quantity > 0).Select(x => new ProductComboItemRequest(x.ProductId, x.Quantity)).ToList());
    private async Task LoadAsync(CancellationToken cancellationToken) { Products = (await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>(); Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>(); }
    private static List<ComboItemInput> Pad(List<ComboItemInput> items) { while (items.Count < ComboRows) items.Add(new ComboItemInput { Quantity = 1 }); return items.Take(ComboRows).ToList(); }
    public class ProductInput { public string Title { get; set; } = string.Empty; public string? Description { get; set; } public decimal Price { get; set; } public decimal SelfPrice { get; set; } public bool IsActive { get; set; } public bool IsCombo { get; set; } public bool UseOwnRecipeForCombo { get; set; } public int? ProductCategoryId { get; set; } public List<ComboItemInput> ComboItems { get; set; } = new(); }
    public class ComboItemInput { public int ProductId { get; set; } public decimal Quantity { get; set; } }
}