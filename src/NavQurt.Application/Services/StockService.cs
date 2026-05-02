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
        var result = await GetBalancesAsync(new StockBalanceRequest(WarehouseId: warehouseId), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<IngredientStockDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<IngredientStockDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<StockBalanceResponse>> GetBalancesAsync(StockBalanceRequest request, CancellationToken cancellationToken = default)
    {
        var warehouseQuery = repository.Query<Warehouse>()
            .Where(x => !x.IsDeleted && x.IsActive)
            .AsQueryable();

        if (request.WarehouseId.HasValue)
        {
            warehouseQuery = warehouseQuery.Where(x => x.Id == request.WarehouseId);
        }

        var warehouses = await warehouseQuery
            .OrderBy(x => x.Title)
            .Select(x => new { x.Id, x.Title })
            .ToListAsync(cancellationToken);

        var ingredientQuery = repository.Query<Ingredient>()
            .Include(x => x.IngredientCategory)
            .Where(x => !x.IsDeleted && x.IsActive)
            .AsQueryable();

        if (request.CategoryId.HasValue)
        {
            ingredientQuery = ingredientQuery.Where(x => x.IngredientCategoryId == request.CategoryId);
        }

        if (!string.IsNullOrWhiteSpace(request.Ingredient))
        {
            var ingredient = request.Ingredient.Trim().ToLower();
            ingredientQuery = ingredientQuery.Where(x => x.Title.ToLower().Contains(ingredient));
        }

        var ingredients = await ingredientQuery
            .OrderBy(x => x.Title)
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Unit,
                x.AverageSelfPrice,
                x.MinRemainderLimit,
                x.IngredientCategoryId,
                CategoryTitle = x.IngredientCategory != null ? x.IngredientCategory.Title : null
            })
            .ToListAsync(cancellationToken);

        var warehouseIds = warehouses.Select(x => x.Id).ToArray();
        var ingredientIds = ingredients.Select(x => x.Id).ToArray();

        var stocks = await repository.Query<IngredientStock>()
            .Where(x => warehouseIds.Contains(x.WarehouseId) && ingredientIds.Contains(x.IngredientId))
            .Select(x => new { x.WarehouseId, x.IngredientId, x.Quantity })
            .ToListAsync(cancellationToken);

        var stockMap = stocks.ToDictionary(x => (x.WarehouseId, x.IngredientId), x => x.Quantity);
        var items = new List<IngredientStockDto>();

        foreach (var warehouse in warehouses)
        {
            foreach (var ingredient in ingredients)
            {
                stockMap.TryGetValue((warehouse.Id, ingredient.Id), out var quantity);
                items.Add(new IngredientStockDto(
                    ingredient.Id,
                    ingredient.Title,
                    ingredient.Unit,
                    warehouse.Id,
                    warehouse.Title,
                    quantity,
                    ingredient.AverageSelfPrice,
                    ingredient.MinRemainderLimit,
                    ingredient.IngredientCategoryId,
                    ingredient.CategoryTitle));
            }
        }

        if (request.OnlyOutOfStock)
        {
            items = items.Where(x => x.Quantity <= 0).ToList();
        }

        var response = new StockBalanceResponse(
            items,
            items.Count,
            items.Sum(x => x.Total),
            items.Where(x => x.Quantity > 0).Sum(x => x.Total));

        return ResponseResult<StockBalanceResponse>.CreateSuccess(response);
    }
}
