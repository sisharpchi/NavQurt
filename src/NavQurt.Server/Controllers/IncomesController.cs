using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/incomes")]
public class IncomesController(IIncomeService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? warehouseId,
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var request = new IncomeListRequest(fromDate, toDate, warehouseId, search);
        return Ok(await service.GetListAsync(request, cancellationToken));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest request, CancellationToken cancellationToken) => Ok(await service.CreateAsync(request, cancellationToken));
}
