using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Common;

internal sealed class RecipeCalculator(IMainRepository repository)
{
    public Task<ResponseResult<Dictionary<int, decimal>>> CalculateForProductAsync(int productId, decimal quantity, CancellationToken cancellationToken = default) =>
        CalculateProductAsync(productId, quantity, new HashSet<int>(), new HashSet<int>(), cancellationToken);

    public Task<ResponseResult<Dictionary<int, decimal>>> CalculateForIngredientAsync(int ingredientId, decimal quantity, CancellationToken cancellationToken = default) =>
        CalculateIngredientAsync(ingredientId, quantity, new HashSet<int>(), cancellationToken);

    private async Task<ResponseResult<Dictionary<int, decimal>>> CalculateProductAsync(
        int productId,
        decimal quantity,
        HashSet<int> productPath,
        HashSet<int> ingredientPath,
        CancellationToken cancellationToken)
    {
        if (!productPath.Add(productId))
        {
            return ResponseResult<Dictionary<int, decimal>>.CreateError("Product combo cycle aniqlandi.", WebErrorConstant.RecipeSelfReference);
        }

        var product = await repository.Query<Product>()
            .Include(x => x.Recipe)
            .ThenInclude(x => x!.Items)
            .Include(x => x.ComboItems)
            .FirstOrDefaultAsync(x => x.Id == productId && !x.IsDeleted, cancellationToken);

        if (product == null)
        {
            return ResponseResult<Dictionary<int, decimal>>.CreateError("Product topilmadi.", WebErrorConstant.ProductNotExists);
        }

        Dictionary<int, decimal> result;
        if (product.IsCombo && !product.UseOwnRecipeForCombo)
        {
            result = new Dictionary<int, decimal>();
            foreach (var comboItem in product.ComboItems)
            {
                var child = await CalculateProductAsync(comboItem.ProductId, quantity * comboItem.Quantity, productPath, ingredientPath, cancellationToken);
                if (!child.Success)
                {
                    return child;
                }

                Merge(result, child.Value);
            }
        }
        else
        {
            if (product.Recipe == null || product.Recipe.IsDeleted)
            {
                return ResponseResult<Dictionary<int, decimal>>.CreateError($"'{product.Title}' productida recipe yo'q.", WebErrorConstant.RecipeNotExists);
            }

            var recipeResult = await CalculateRecipeItemsAsync(product.Recipe, quantity, ingredientPath, cancellationToken);
            if (!recipeResult.Success)
            {
                return recipeResult;
            }

            result = recipeResult.Value;
        }

        productPath.Remove(productId);
        return ResponseResult<Dictionary<int, decimal>>.CreateSuccess(result);
    }

    private async Task<ResponseResult<Dictionary<int, decimal>>> CalculateIngredientAsync(
        int ingredientId,
        decimal quantity,
        HashSet<int> ingredientPath,
        CancellationToken cancellationToken)
    {
        if (!ingredientPath.Add(ingredientId))
        {
            return ResponseResult<Dictionary<int, decimal>>.CreateError("Ingredient recipe cycle aniqlandi.", WebErrorConstant.RecipeSelfReference);
        }

        var ingredient = await repository.Query<Ingredient>()
            .Include(x => x.Recipe)
            .ThenInclude(x => x!.Items)
            .FirstOrDefaultAsync(x => x.Id == ingredientId && !x.IsDeleted, cancellationToken);

        if (ingredient == null)
        {
            return ResponseResult<Dictionary<int, decimal>>.CreateError("Ingredient topilmadi.", WebErrorConstant.IngredientNotExists);
        }

        if (ingredient.Recipe == null || ingredient.Recipe.IsDeleted)
        {
            ingredientPath.Remove(ingredientId);
            return ResponseResult<Dictionary<int, decimal>>.CreateSuccess(new Dictionary<int, decimal> { [ingredientId] = quantity });
        }

        var recipeResult = await CalculateRecipeItemsAsync(ingredient.Recipe, quantity, ingredientPath, cancellationToken);
        if (!recipeResult.Success)
        {
            return recipeResult;
        }

        var result = recipeResult.Value;
        ingredientPath.Remove(ingredientId);
        return ResponseResult<Dictionary<int, decimal>>.CreateSuccess(result);
    }

    private async Task<ResponseResult<Dictionary<int, decimal>>> CalculateRecipeItemsAsync(
        Recipe recipe,
        decimal multiplier,
        HashSet<int> ingredientPath,
        CancellationToken cancellationToken)
    {
        var result = new Dictionary<int, decimal>();
        foreach (var item in recipe.Items)
        {
            var child = await CalculateIngredientAsync(item.IngredientId, item.Quantity * multiplier / recipe.PortionYield, ingredientPath, cancellationToken);
            if (!child.Success)
            {
                return child;
            }

            Merge(result, child.Value);
        }

        return ResponseResult<Dictionary<int, decimal>>.CreateSuccess(result);
    }

    private static void Merge(IDictionary<int, decimal> target, IReadOnlyDictionary<int, decimal>? source)
    {
        if (source == null)
        {
            return;
        }

        foreach (var (ingredientId, quantity) in source)
        {
            target[ingredientId] = target.TryGetValue(ingredientId, out var current) ? current + quantity : quantity;
        }
    }
}
