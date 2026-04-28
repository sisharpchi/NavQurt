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
        var items = await repository.Query<Customer>(x => !x.IsDeleted)
            .OrderBy(x => x.FullName)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<CustomerDto>>.CreateSuccess(items);
    }
}
