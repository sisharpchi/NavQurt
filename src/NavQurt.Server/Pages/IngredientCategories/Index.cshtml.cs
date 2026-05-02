using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.IngredientCategories;

public class IndexModel(IIngredientCategoryService service) : PageModel
{
    public IReadOnlyCollection<IngredientCategoryDto> Items { get; private set; } = Array.Empty<IngredientCategoryDto>();
    public int ItemsCount { get; private set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Status { get; set; }

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(CancellationToken cancellationToken)
    {
        var result = await service.GetListAsync(new IngredientCategoryListRequest(Search, ParseBool(Status)), cancellationToken);
        Items = result.Value?.Items ?? Array.Empty<IngredientCategoryDto>();
        ItemsCount = result.Value?.ItemsCount ?? Items.Count;
    }

    private static bool? ParseBool(string? value) =>
        bool.TryParse(value, out var parsed) ? parsed : null;
}
