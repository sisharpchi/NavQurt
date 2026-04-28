using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.ProductCategories;

public class IndexModel(IProductCategoryService service) : PageModel
{
    [BindProperty]
    public ProductCategoryInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<ProductCategoryDto> Categories { get; private set; } = Array.Empty<ProductCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Categories.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new ProductCategoryInput
            {
                Id = item.Id,
                Title = item.Title,
                ParentCategoryId = item.ParentCategoryId,
                IsActive = item.IsActive,
                Priority = item.Priority
            };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new ProductCategoryRequest(Input.Title, Input.ParentCategoryId, Input.IsActive, Input.Priority);
        var result = Input.Id > 0
            ? await service.UpdateAsync(Input.Id, request, cancellationToken)
            : await service.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Category saqlandi." : result.Error;
        return RedirectToPage();
    }

    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var result = await service.GetListAsync(cancellationToken);
        Categories = result.Value ?? Array.Empty<ProductCategoryDto>();
    }

    public class ProductCategoryInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
        public bool IsActive { get; set; }
        public int Priority { get; set; }
    }
}
