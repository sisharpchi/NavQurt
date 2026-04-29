using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.IngredientCategories;

public class DetailModel(IIngredientCategoryService service) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new();

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnGet(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(id, cancellationToken);
        if (!result.Success) return NotFound();

        Input = new InputModel { Title = result.Value.Title, IsActive = result.Value.IsActive };
        return Page();
    }

    public async Task<IActionResult> OnPost(int id, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, new IngredientCategoryRequest(Input.Title, Input.IsActive), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        Message = "Yozuv saqlandi.";
        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}