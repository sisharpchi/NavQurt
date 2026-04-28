using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/recipes")]
public class RecipesController(IRecipeService service) : ControllerBase
{
    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId, CancellationToken cancellationToken) => Ok(await service.GetByProductAsync(productId, cancellationToken));

    [HttpGet("ingredient/{ingredientId:int}")]
    public async Task<IActionResult> GetByIngredient(int ingredientId, CancellationToken cancellationToken) => Ok(await service.GetByIngredientAsync(ingredientId, cancellationToken));

    [HttpPut]
    public async Task<IActionResult> Upsert(UpsertRecipeRequest request, CancellationToken cancellationToken) => Ok(await service.UpsertAsync(request, cancellationToken));
}
