using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Products;

public class IndexModel(IProductService productService, IProductCategoryService categoryService) : PageModel
{
    private const int ComboRows = 5;

    [BindProperty]
    public ProductInput Input { get; set; } = CreateEmptyInput();

    public IReadOnlyCollection<ProductDto> Products { get; private set; } = Array.Empty<ProductDto>();
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Products.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new ProductInput
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Price = item.Price,
                SelfPrice = item.SelfPrice,
                IsActive = item.IsActive,
                IsCombo = item.IsCombo,
                UseOwnRecipeForCombo = item.UseOwnRecipeForCombo,
                ProductCategoryId = item.ProductCategoryId,
                ComboItems = PadComboItems(item.ComboItems.Select(x => new ComboItemInput { ProductId = x.ProductId, Quantity = x.Quantity }).ToList())
            };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var comboItems = Input.ComboItems
            .Where(x => x.ProductId > 0 && x.Quantity > 0)
            .Select(x => new ProductComboItemRequest(x.ProductId, x.Quantity))
            .ToList();

        var request = new ProductRequest(
            Input.Title,
            Input.Description,
            Input.Price,
            Input.SelfPrice,
            Input.IsActive,
            Input.IsCombo,
            Input.UseOwnRecipeForCombo,
            Input.ProductCategoryId,
            comboItems);

        var result = Input.Id > 0
            ? await productService.UpdateAsync(Input.Id, request, cancellationToken)
            : await productService.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Product saqlandi." : result.Error;
        return RedirectToPage();
    }

    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Products = (await productService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductDto>();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
    }

    private static ProductInput CreateEmptyInput() => new()
    {
        IsActive = true,
        ComboItems = PadComboItems(new List<ComboItemInput>())
    };

    private static List<ComboItemInput> PadComboItems(List<ComboItemInput> items)
    {
        while (items.Count < ComboRows)
        {
            items.Add(new ComboItemInput { Quantity = 1 });
        }

        return items.Take(ComboRows).ToList();
    }

    public class ProductInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal SelfPrice { get; set; }
        public bool IsActive { get; set; }
        public bool IsCombo { get; set; }
        public bool UseOwnRecipeForCombo { get; set; }
        public int? ProductCategoryId { get; set; }
        public List<ComboItemInput> ComboItems { get; set; } = new();
    }

    public class ComboItemInput
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}
