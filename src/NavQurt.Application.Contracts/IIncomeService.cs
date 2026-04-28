using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IIncomeService
{
    Task<ResponseResult<IncomeDto>> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<IReadOnlyCollection<IncomeDto>>> GetListAsync(CancellationToken cancellationToken = default);
}
