using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Enumerations;
using NavQurt.Core.Persistence;

namespace NavQurt.Application.Common;

internal sealed class StockMutationService(IMainRepository repository)
{
    public async Task ApplyAsync(
        int warehouseId,
        int ingredientId,
        decimal quantity,
        StockMovementType type,
        int? incomeId,
        int? orderId,
        string? comment,
        CancellationToken cancellationToken)
    {
        var stock = await repository.Query<IngredientStock>()
            .FirstOrDefaultAsync(x => x.WarehouseId == warehouseId && x.IngredientId == ingredientId, cancellationToken);

        if (stock == null)
        {
            stock = new IngredientStock { WarehouseId = warehouseId, IngredientId = ingredientId };
            await repository.AddAsync(stock);
        }

        stock.Quantity += quantity;
        stock.UpdatedAt = DateTime.UtcNow;

        await repository.AddAsync(new StockMovement
        {
            WarehouseId = warehouseId,
            IngredientId = ingredientId,
            Quantity = quantity,
            BalanceAfter = stock.Quantity,
            Type = type,
            IncomeId = incomeId,
            OrderId = orderId,
            Comment = comment
        });
    }
}
