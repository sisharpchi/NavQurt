using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IProductService
{
    Task<ResponseResult<IReadOnlyCollection<ProductDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<ListResponse<ProductDto>>> GetListAsync(ProductListRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<ProductDto>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ResponseResult<ProductDto>> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<ProductDto>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
