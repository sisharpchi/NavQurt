using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.ProductCategories;

public class CreateModel(IProductCategoryService service) : PageModel
{
    [BindProperty]
    public ProductCategoryInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(new ProductCategoryRequest(Input.Title, Input.ParentCategoryId, Input.IsActive, Input.Priority), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            await LoadAsync(cancellationToken);
            return Page();
        }

        Message = "Category yaratildi.";
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