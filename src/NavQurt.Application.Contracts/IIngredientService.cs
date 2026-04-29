using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IIngredientService
{
    Task<ResponseResult<IReadOnlyCollection<IngredientDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientDto>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientDto>> CreateAsync(IngredientRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<IngredientDto>> UpdateAsync(int id, IngredientRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
