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
        var items = await repository.Query<PaymentMethod>(x => !x.IsDeleted)
            .OrderBy(x => x.Title)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<PaymentMethodDto>>.CreateSuccess(items);
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
