using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IRecipeService
{
    Task<ResponseResult<RecipeDto>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> GetByIngredientAsync(int ingredientId, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> UpsertAsync(UpsertRecipeRequest request, CancellationToken cancellationToken = default);
}
