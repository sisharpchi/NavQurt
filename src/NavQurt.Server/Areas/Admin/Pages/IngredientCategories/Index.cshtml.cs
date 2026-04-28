using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.IngredientCategories;

public class IndexModel(IIngredientCategoryService service) : PageModel
{
    [BindProperty]
    public IngredientCategoryInput Input { get; set; } = new() { IsActive = true };

    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Categories.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new IngredientCategoryInput { Id = item.Id, Title = item.Title, IsActive = item.IsActive };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new IngredientCategoryRequest(Input.Title, Input.IsActive);
        var result = Input.Id > 0
            ? await service.UpdateAsync(Input.Id, request, cancellationToken)
            : await service.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Category saqlandi." : result.Error;
        return RedirectToPage();
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var result = await service.GetListAsync(cancellationToken);
        Categories = result.Value ?? Array.Empty<IngredientCategoryDto>();
    }

    public class IngredientCategoryInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
