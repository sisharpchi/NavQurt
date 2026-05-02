using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface ICustomerService
{
    Task<ResponseResult<IReadOnlyCollection<CustomerDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<ListResponse<CustomerDto>>> GetListAsync(CustomerListRequest request, CancellationToken cancellationToken = default);
}
