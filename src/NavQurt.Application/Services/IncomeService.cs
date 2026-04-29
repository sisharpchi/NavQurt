using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Enumerations;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class IncomeService(IMainRepository repository, StockMutationService stockMutation) : BusinessServiceBase, IIncomeService
{
    public async Task<ResponseResult<IReadOnlyCollection<IncomeDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.Query<Income>()
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<IncomeDto>>.CreateSuccess(items.Select(x => x.ToDto()).ToList());
    }

    public async Task<ResponseResult<IncomeDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.Query<Income>()
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity == null ? NotFound<IncomeDto>("Income") : ResponseResult<IncomeDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<IncomeDto>> CreateAsync(CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest<IncomeDto>("Income itemlar bo'sh bo'lmasligi kerak.");
        }

        if (!await repository.Query<Warehouse>().AnyAsync(x => x.Id == request.WarehouseId && !x.IsDeleted && x.IsActive, cancellationToken))
        {
            return ResponseResult<IncomeDto>.CreateError("Warehouse topilmadi.", WebErrorConstant.WarehouseNotExists);
        }

        var ingredientIds = request.Items.Select(x => x.IngredientId).Distinct().ToArray();
        var foundIngredients = await repository.Query<Ingredient>().CountAsync(x => ingredientIds.Contains(x.Id) && !x.IsDeleted, cancellationToken);
        if (foundIngredients != ingredientIds.Length)
        {
            return ResponseResult<IncomeDto>.CreateError("Ingredient topilmadi.", WebErrorConstant.IngredientNotExists);
        }

        if (request.Items.Any(x => x.Quantity <= 0 || x.Price < 0))
        {
            return BadRequest<IncomeDto>("Income quantity 0 dan katta, price esa 0 yoki katta bo'lishi kerak.");
        }

        await using var transaction = await repository.Database.BeginTransactionAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var income = new Income
        {
            WarehouseId = request.WarehouseId,
            CreatedAt = now,
            AcceptedAt = now,
            Comment = request.Comment,
            TotalAmount = request.Items.Sum(x => x.Quantity * x.Price)
        };

        foreach (var item in request.Items)
        {
            income.Items.Add(new IncomeItem
            {
                IngredientId = item.IngredientId,
                Quantity = item.Quantity,
                Price = item.Price
            });
        }

        await repository.AddAsync(income);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        foreach (var item in income.Items)
        {
            await stockMutation.ApplyAsync(request.WarehouseId, item.IngredientId, item.Quantity, StockMovementType.Income, income.Id, null, "Income", cancellationToken);
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        income = await repository.Query<Income>()
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient)
            .FirstAsync(x => x.Id == income.Id, cancellationToken);

        return ResponseResult<IncomeDto>.CreateSuccess(income.ToDto());
    }
}
