using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IOrderService
{
    Task<ResponseResult<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<IReadOnlyCollection<OrderDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<OrderListResponse>> GetListAsync(OrderListRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<OrderDto>> GetAsync(int id, CancellationToken cancellationToken = default);
}
