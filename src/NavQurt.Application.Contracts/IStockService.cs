using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IStockService
{
    Task<ResponseResult<IReadOnlyCollection<IngredientStockDto>>> GetBalancesAsync(int? warehouseId = default, CancellationToken cancellationToken = default);
}
