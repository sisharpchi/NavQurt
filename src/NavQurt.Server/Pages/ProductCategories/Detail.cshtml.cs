using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.ProductCategories;

public class DetailModel(IProductCategoryService service) : PageModel
{
    [BindProperty]
    public ProductCategoryInput Input { get; set; } = new();

    public int Id { get; private set; }
    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        Id = id;
        await LoadAsync(cancellationToken);
        var result = await service.GetAsync(id, cancellationToken);
        if (!result.Success)
        {
            return NotFound();
        }

        Input = new ProductCategoryInput
        {
            Title = result.Value.Title,
            ParentCategoryId = result.Value.ParentCategoryId,
            IsActive = result.Value.IsActive,
            Priority = result.Value.Priority
        };
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        Id = id;
        var result = await service.UpdateAsync(id, new ProductCategoryRequest(Input.Title, Input.ParentCategoryId, Input.IsActive, Input.Priority), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            await LoadAsync(cancellationToken);
            return Page();
        }

        Message = "Category saqlandi.";
        return RedirectToPage("./Index");
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Categories = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<ProductCategoryDto>();
    }

    public class ProductCategoryInput
    {
        public string Title { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }
}