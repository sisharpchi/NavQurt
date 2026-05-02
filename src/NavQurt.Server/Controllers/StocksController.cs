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
    public async Task<IActionResult> GetBalances(
        [FromQuery] int? warehouseId,
        [FromQuery] int? categoryId,
        [FromQuery] string? ingredient,
        [FromQuery] bool onlyOutOfStock,
        CancellationToken cancellationToken)
    {
        var request = new StockBalanceRequest(warehouseId, categoryId, ingredient, onlyOutOfStock);
        return Ok(await service.GetBalancesAsync(request, cancellationToken));
    }
}
