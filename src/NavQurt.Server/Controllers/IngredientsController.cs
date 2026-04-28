using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/ingredients")]
public class IngredientsController(IIngredientService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(CancellationToken cancellationToken) => Ok(await service.GetListAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(IngredientRequest request, CancellationToken cancellationToken) => Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, IngredientRequest request, CancellationToken cancellationToken) => Ok(await service.UpdateAsync(id, request, cancellationToken));
}
