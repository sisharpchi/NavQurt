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
    public async Task<IActionResult> GetList(CancellationToken cancellationToken) => Ok(await service.GetListAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create(CreateIncomeRequest request, CancellationToken cancellationToken) => Ok(await service.CreateAsync(request, cancellationToken));
}
