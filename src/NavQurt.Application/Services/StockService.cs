using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class StockService(IMainRepository repository) : IStockService
{
    public async Task<ResponseResult<IReadOnlyCollection<IngredientStockDto>>> GetBalancesAsync(int? warehouseId = default, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<IngredientStock>()
            .Include(x => x.Ingredient)
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (warehouseId.HasValue)
        {
            query = query.Where(x => x.WarehouseId == warehouseId);
        }

        var items = await query
            .OrderBy(x => x.Warehouse.Title)
            .ThenBy(x => x.Ingredient.Title)
            .Select(x => new IngredientStockDto(x.IngredientId, x.Ingredient.Title, x.Ingredient.Unit, x.WarehouseId, x.Warehouse.Title, x.Quantity))
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<IngredientStockDto>>.CreateSuccess(items);
    }
}
