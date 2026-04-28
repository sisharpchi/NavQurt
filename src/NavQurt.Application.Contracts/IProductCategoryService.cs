using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IProductCategoryService
{
    Task<ResponseResult<IReadOnlyCollection<ProductCategoryDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<ProductCategoryDto>> CreateAsync(ProductCategoryRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<ProductCategoryDto>> UpdateAsync(int id, ProductCategoryRequest request, CancellationToken cancellationToken = default);
}
