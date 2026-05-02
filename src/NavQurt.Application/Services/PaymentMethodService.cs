using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class PaymentMethodService(IMainRepository repository) : BusinessServiceBase, IPaymentMethodService
{
    public async Task<ResponseResult<IReadOnlyCollection<PaymentMethodDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new PaymentMethodListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<PaymentMethodDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<PaymentMethodDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<PaymentMethodDto>>> GetListAsync(PaymentMethodListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<PaymentMethod>(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var items = await query
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<PaymentMethodDto>>.CreateSuccess(new ListResponse<PaymentMethodDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<PaymentMethodDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<PaymentMethod>(x => x.Id == id && !x.IsDeleted);
        return entity == null ? NotFound<PaymentMethodDto>("Payment method") : ResponseResult<PaymentMethodDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<PaymentMethodDto>> CreateAsync(PaymentMethodRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasText(request.Title))
        {
            return BadRequest<PaymentMethodDto>("Payment method nomi majburiy.");
        }

        var entity = new PaymentMethod { Title = request.Title.Trim(), IsActive = request.IsActive };
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<PaymentMethodDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<PaymentMethodDto>> UpdateAsync(int id, PaymentMethodRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<PaymentMethod>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<PaymentMethodDto>("Payment method");
        }

        if (!HasText(request.Title))
        {
            return BadRequest<PaymentMethodDto>("Payment method nomi majburiy.");
        }

        entity.Title = request.Title.Trim();
        entity.IsActive = request.IsActive;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<PaymentMethodDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<PaymentMethod>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Payment method");
        }

        entity.IsDeleted = true;
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
