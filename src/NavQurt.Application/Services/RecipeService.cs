using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class RecipeService(IMainRepository repository, RecipeCalculator calculator) : BusinessServiceBase, IRecipeService
{
    public async Task<ResponseResult<RecipeDto>> GetByProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        var recipe = await RecipeQuery().FirstOrDefaultAsync(x => x.ProductId == productId && !x.IsDeleted, cancellationToken);
        return recipe == null ? NotFound<RecipeDto>("Recipe") : ResponseResult<RecipeDto>.CreateSuccess(recipe.ToDto());
    }

    public async Task<ResponseResult<RecipeDto>> GetByIngredientAsync(int ingredientId, CancellationToken cancellationToken = default)
    {
        var recipe = await RecipeQuery().FirstOrDefaultAsync(x => x.IngredientId == ingredientId && !x.IsDeleted, cancellationToken);
        return recipe == null ? NotFound<RecipeDto>("Recipe") : ResponseResult<RecipeDto>.CreateSuccess(recipe.ToDto());
    }

    public async Task<ResponseResult<RecipeDto>> UpsertAsync(UpsertRecipeRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<RecipeDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        await using var transaction = await repository.Database.BeginTransactionAsync(cancellationToken);

        var recipe = await RecipeQuery()
            .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.IngredientId == request.IngredientId, cancellationToken);

        if (recipe == null)
        {
            recipe = new Recipe { ProductId = request.ProductId, IngredientId = request.IngredientId };
            await repository.AddAsync(recipe);
        }

        recipe.PortionYield = request.PortionYield;
        recipe.IsDeleted = false;
        recipe.Items.Clear();
        foreach (var item in request.Items)
        {
            recipe.Items.Add(new RecipeItem { IngredientId = item.IngredientId, Quantity = item.Quantity });
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);

        var cycleCheck = request.ProductId.HasValue
            ? await calculator.CalculateForProductAsync(request.ProductId.Value, 1, cancellationToken)
            : await calculator.CalculateForIngredientAsync(request.IngredientId!.Value, 1, cancellationToken);

        if (!cycleCheck.Success)
        {
            await transaction.RollbackAsync(cancellationToken);
            return ResponseResult<RecipeDto>.CreateError(cycleCheck.Error!, cycleCheck.ErrorCode);
        }

        recipe = await RecipeQuery().FirstAsync(x => x.Id == recipe.Id, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ResponseResult<RecipeDto>.CreateSuccess(recipe.ToDto());
    }

    private IQueryable<Recipe> RecipeQuery() =>
        repository.Query<Recipe>()
            .Include(x => x.Items)
            .ThenInclude(x => x.Ingredient);

    private async Task<ResponseResult> ValidateAsync(UpsertRecipeRequest request, CancellationToken cancellationToken)
    {
        if ((request.ProductId.HasValue && request.IngredientId.HasValue) || (!request.ProductId.HasValue && !request.IngredientId.HasValue))
        {
            return ResponseResult.CreateError("Recipe faqat bitta product yoki ingredientga tegishli bo'lishi kerak.", WebErrorConstant.RecipeInvalidReference);
        }

        if (request.PortionYield <= 0)
        {
            return ResponseResult.CreateError("Recipe portion yield 0 dan katta bo'lishi kerak.", WebErrorConstant.RecipeInvalidPortion);
        }

        if (request.Items.Count == 0)
        {
            return ResponseResult.CreateError("Recipe itemlar bo'sh bo'lmasligi kerak.", WebErrorConstant.RecipeInvalidReference);
        }

        if (request.Items.Any(x => x.Quantity <= 0))
        {
            return ResponseResult.CreateError("Recipe item quantity 0 dan katta bo'lishi kerak.", WebErrorConstant.RecipeInvalidPortion);
        }

        if (request.Items.Select(x => x.IngredientId).Distinct().Count() != request.Items.Count)
        {
            return ResponseResult.CreateError("Recipe ichida duplicate ingredient bor.", WebErrorConstant.RecipeDuplicateIngredient);
        }

        if (request.ProductId.HasValue && !await repository.Query<Product>().AnyAsync(x => x.Id == request.ProductId && !x.IsDeleted, cancellationToken))
        {
            return ResponseResult.CreateError("Product topilmadi.", WebErrorConstant.ProductNotExists);
        }

        if (request.IngredientId.HasValue && !await repository.Query<Ingredient>().AnyAsync(x => x.Id == request.IngredientId && !x.IsDeleted, cancellationToken))
        {
            return ResponseResult.CreateError("Ingredient topilmadi.", WebErrorConstant.IngredientNotExists);
        }

        var itemIds = request.Items.Select(x => x.IngredientId).ToArray();
        var foundCount = await repository.Query<Ingredient>().CountAsync(x => itemIds.Contains(x.Id) && !x.IsDeleted, cancellationToken);
        if (foundCount != itemIds.Length)
        {
            return ResponseResult.CreateError("Recipe item ingredient topilmadi.", WebErrorConstant.IngredientNotExists);
        }

        if (request.IngredientId.HasValue && itemIds.Contains(request.IngredientId.Value))
        {
            return ResponseResult.CreateError("Ingredient recipe o'zini ishlata olmaydi.", WebErrorConstant.RecipeSelfReference);
        }

        return ResponseResult.CreateSuccess();
    }
}
