using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IIngredientCategoryService
{
    Task<ResponseResult<IReadOnlyCollection<IngredientCategoryDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<ListResponse<IngredientCategoryDto>>> GetListAsync(IngredientCategoryListRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientCategoryDto>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientCategoryDto>> CreateAsync(IngredientCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientCategoryDto>> UpdateAsync(int id, IngredientCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
