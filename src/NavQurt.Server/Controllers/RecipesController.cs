using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/recipes")]
public class RecipesController(IRecipeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] RecipeListRequest request, CancellationToken cancellationToken) => Ok(await service.GetListAsync(request, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken) => Ok(await service.GetAsync(id, cancellationToken));

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId, CancellationToken cancellationToken) => Ok(await service.GetByProductAsync(productId, cancellationToken));

    [HttpGet("ingredient/{ingredientId:int}")]
    public async Task<IActionResult> GetByIngredient(int ingredientId, CancellationToken cancellationToken) => Ok(await service.GetByIngredientAsync(ingredientId, cancellationToken));

    [HttpPut]
    public async Task<IActionResult> Upsert(UpsertRecipeRequest request, CancellationToken cancellationToken) => Ok(await service.UpsertAsync(request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) => Ok(await service.DeleteAsync(id, cancellationToken));
}
