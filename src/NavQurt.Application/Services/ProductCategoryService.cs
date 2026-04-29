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
        var items = await repository.Query<ProductCategory>(x => !x.IsDeleted)
            .OrderBy(x => x.Priority)
            .ThenBy(x => x.Title)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<ProductCategoryDto>>.CreateSuccess(items);
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

        if (request.ParentCategoryId == id)
        {
            return BadRequest<ProductCategoryDto>("Kategoriya o'ziga parent bo'la olmaydi.");
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
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
