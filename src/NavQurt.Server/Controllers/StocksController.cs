using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NavQurt.Application.Contracts;

namespace NavQurt.Server.Controllers;

[ApiController]
[Authorize]
[Route("api/stocks")]
public class StocksController(IStockService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBalances([FromQuery] int? warehouseId, CancellationToken cancellationToken) => Ok(await service.GetBalancesAsync(warehouseId, cancellationToken));
}
