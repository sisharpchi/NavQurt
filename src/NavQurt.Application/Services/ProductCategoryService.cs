using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class ProductCategoryService(IMainRepository repository) : BusinessServiceBase, IProductCategoryService
{
    public async Task<ResponseResult<IReadOnlyCollection<ProductCategoryDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new ProductCategoryListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<ProductCategoryDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<ProductCategoryDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<ProductCategoryDto>>> GetListAsync(ProductCategoryListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<ProductCategory>(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.ParentCategoryId.HasValue)
        {
            query = query.Where(x => x.ParentCategoryId == request.ParentCategoryId.Value);
        }

        var items = await query
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<ProductCategoryDto>>.CreateSuccess(new ListResponse<ProductCategoryDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<ProductCategoryDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<ProductCategory>(x => x.Id == id && !x.IsDeleted);
        return entity == null ? NotFound<ProductCategoryDto>("Kategoriya") : ResponseResult<ProductCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<ProductCategoryDto>> CreateAsync(ProductCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasText(request.Title))
        {
            return BadRequest<ProductCategoryDto>("Kategoriya nomi majburiy.");
        }

        if (request.ParentCategoryId.HasValue && !await repository.Query<ProductCategory>().AnyAsync(x => x.Id == request.ParentCategoryId && !x.IsDeleted, cancellationToken))
        {
            return NotFound<ProductCategoryDto>("Parent kategoriya");
        }

        var entity = new ProductCategory
        {
            Title = request.Title.Trim(),
            ParentCategoryId = request.ParentCategoryId,
            IsActive = request.IsActive,
            Priority = request.Priority
        };

        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return ResponseResult<ProductCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<ProductCategoryDto>> UpdateAsync(int id, ProductCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<ProductCategory>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<ProductCategoryDto>("Kategoriya");
        }

        if (!HasText(request.Title))
        {
            return BadRequest<ProductCategoryDto>("Kategoriya nomi majburiy.");
        }

        if (request.ParentCategoryId == id)
        {
            return BadRequest<ProductCategoryDto>("Kategoriya o'ziga parent bo'la olmaydi.");
        }

        if (request.ParentCategoryId.HasValue && !await repository.Query<ProductCategory>().AnyAsync(x => x.Id == request.ParentCategoryId && !x.IsDeleted, cancellationToken))
        {
            return NotFound<ProductCategoryDto>("Parent kategoriya");
        }

        entity.Title = request.Title.Trim();
        entity.ParentCategoryId = request.ParentCategoryId;
        entity.IsActive = request.IsActive;
        entity.Priority = request.Priority;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<ProductCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<ProductCategory>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Kategoriya");
        }

        entity.IsDeleted = true;
        var products = await repository.Query<Product>().Where(x => x.ProductCategoryId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var product in products)
        {
            product.IsDeleted = true;
        }

        var children = await repository.Query<ProductCategory>().Where(x => x.ParentCategoryId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var child in children)
        {
            child.ParentCategoryId = null;
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
