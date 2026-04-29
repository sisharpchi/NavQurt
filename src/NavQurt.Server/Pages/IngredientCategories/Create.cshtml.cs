using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Pages.IngredientCategories;

public class CreateModel(IIngredientCategoryService service) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new() { IsActive = true };

    [TempData]
    public string? Message { get; set; }

    public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(new IngredientCategoryRequest(Input.Title, Input.IsActive), cancellationToken);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error);
            return Page();
        }

        Message = "Yozuv yaratildi.";
        return RedirectToPage("./Index");
    }

    public class InputModel
    {
        public string Title { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}