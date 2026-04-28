using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Areas.Admin.Pages.Ingredients;

public class IndexModel(IIngredientService ingredientService, IIngredientCategoryService categoryService) : PageModel
{
    [BindProperty]
    public IngredientInput Input { get; set; } = new() { Unit = "kg", IsActive = true };

    public IReadOnlyCollection<IngredientDto> Ingredients { get; private set; } = Array.Empty<IngredientDto>();
    public IReadOnlyCollection<IngredientCategoryDto> Categories { get; private set; } = Array.Empty<IngredientCategoryDto>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGet(int? editId, CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
        var item = Ingredients.FirstOrDefault(x => x.Id == editId);
        if (item != null)
        {
            Input = new IngredientInput
            {
                Id = item.Id,
                Title = item.Title,
                Unit = item.Unit,
                MinRemainderLimit = item.MinRemainderLimit,
                AverageSelfPrice = item.AverageSelfPrice,
                IsActive = item.IsActive,
                IngredientCategoryId = item.IngredientCategoryId
            };
        }
    }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var request = new IngredientRequest(Input.Title, Input.Unit, Input.MinRemainderLimit, Input.AverageSelfPrice, Input.IsActive, Input.IngredientCategoryId);
        var result = Input.Id > 0
            ? await ingredientService.UpdateAsync(Input.Id, request, cancellationToken)
            : await ingredientService.CreateAsync(request, cancellationToken);

        Message = result.Success ? "Ingredient saqlandi." : result.Error;
        return RedirectToPage();
    }

    public string GetCategoryTitle(int? id) => id.HasValue ? Categories.FirstOrDefault(x => x.Id == id.Value)?.Title ?? "-" : "-";

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Ingredients = (await ingredientService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientDto>();
        Categories = (await categoryService.GetListAsync(cancellationToken)).Value ?? Array.Empty<IngredientCategoryDto>();
    }

    public class IngredientInput
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal MinRemainderLimit { get; set; }
        public decimal AverageSelfPrice { get; set; }
        public bool IsActive { get; set; }
        public int? IngredientCategoryId { get; set; }
    }
}
