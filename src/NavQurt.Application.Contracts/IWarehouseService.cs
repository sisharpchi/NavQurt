using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IWarehouseService
{
    Task<ResponseResult<IReadOnlyCollection<WarehouseDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<WarehouseDto>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ResponseResult<WarehouseDto>> CreateAsync(WarehouseRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<WarehouseDto>> UpdateAsync(int id, WarehouseRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
