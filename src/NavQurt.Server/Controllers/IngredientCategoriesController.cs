using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/ingredient-categories")]
public class IngredientCategoriesController(IIngredientCategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] IngredientCategoryListRequest request, CancellationToken cancellationToken) => Ok(await service.GetListAsync(request, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken) => Ok(await service.GetAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(IngredientCategoryRequest request, CancellationToken cancellationToken) => Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IngredientCategoryRequest request, CancellationToken cancellationToken) => Ok(await service.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) => Ok(await service.DeleteAsync(id, cancellationToken));
}
