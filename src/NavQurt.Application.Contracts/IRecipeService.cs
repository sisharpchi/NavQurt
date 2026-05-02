using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IRecipeService
{
    Task<ResponseResult<RecipeListResponse>> GetListAsync(RecipeListRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> GetByProductAsync(int productId, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> GetByIngredientAsync(int ingredientId, CancellationToken cancellationToken = default);
    Task<ResponseResult<RecipeDto>> UpsertAsync(UpsertRecipeRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
