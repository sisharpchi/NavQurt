using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IPaymentMethodService
{
    Task<ResponseResult<IReadOnlyCollection<PaymentMethodDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<PaymentMethodDto>> CreateAsync(PaymentMethodRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<PaymentMethodDto>> UpdateAsync(int id, PaymentMethodRequest request, CancellationToken cancellationToken = default);
}
