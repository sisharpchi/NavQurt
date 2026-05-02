using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/product-categories")]
public class ProductCategoriesController(IProductCategoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] ProductCategoryListRequest request, CancellationToken cancellationToken) => Ok(await service.GetListAsync(request, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken) => Ok(await service.GetAsync(id, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(ProductCategoryRequest request, CancellationToken cancellationToken) => Ok(await service.CreateAsync(request, cancellationToken));

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductCategoryRequest request, CancellationToken cancellationToken) => Ok(await service.UpdateAsync(id, request, cancellationToken));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) => Ok(await service.DeleteAsync(id, cancellationToken));
}
