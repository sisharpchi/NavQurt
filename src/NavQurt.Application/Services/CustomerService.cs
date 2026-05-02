using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class CustomerService(IMainRepository repository) : ICustomerService
{
    public async Task<ResponseResult<IReadOnlyCollection<CustomerDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new CustomerListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<CustomerDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<CustomerDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<CustomerDto>>> GetListAsync(CustomerListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<Customer>(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.FullName.ToLower().Contains(search) ||
                x.PhoneNumber.ToLower().Contains(search) ||
                (x.Location != null && x.Location.ToLower().Contains(search)));
        }

        var items = await query
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<CustomerDto>>.CreateSuccess(new ListResponse<CustomerDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }
}
