using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.IngredientCategories;

public class IndexModel(IIngredientCategoryService service) : PageModel
{
    public IReadOnlyCollection<IngredientCategoryDto> Items { get; private set; } = Array.Empty<IngredientCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        Items = (await service.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    }
}
