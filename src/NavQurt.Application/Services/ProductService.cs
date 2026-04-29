using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class ProductService(IMainRepository repository) : BusinessServiceBase, IProductService
{
    public async Task<ResponseResult<IReadOnlyCollection<ProductDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await ProductQuery()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<ProductDto>>.CreateSuccess(items.Select(x => x.ToDto()).ToList());
    }

    public async Task<ResponseResult<ProductDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await ProductQuery().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return entity == null
            ? NotFound<ProductDto>("Product")
            : ResponseResult<ProductDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<ProductDto>> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<ProductDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        var entity = new Product();
        Apply(entity, request);
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        entity = await ProductQuery().FirstAsync(x => x.Id == entity.Id, cancellationToken);
        return ResponseResult<ProductDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<ProductDto>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await ProductQuery().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity == null)
        {
            return NotFound<ProductDto>("Product");
        }

        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<ProductDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        Apply(entity, request);
        entity.ComboItems.Clear();
        foreach (var item in request.ComboItems ?? Array.Empty<ProductComboItemRequest>())
        {
            entity.ComboItems.Add(new ProductComboItem { ProductId = item.ProductId, Quantity = item.Quantity });
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        entity = await ProductQuery().FirstAsync(x => x.Id == entity.Id, cancellationToken);
        return ResponseResult<ProductDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Product>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Product");
        }

        entity.IsDeleted = true;
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }

    private IQueryable<Product> ProductQuery() =>
        repository.Query<Product>()
            .Include(x => x.ComboItems)
            .ThenInclude(x => x.Product);

    private async Task<ResponseResult> ValidateAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        if (!HasText(request.Title))
        {
            return ResponseResult.CreateError("Product nomi majburiy.");
        }

        if (request.Price < 0 || request.SelfPrice < 0)
        {
            return ResponseResult.CreateError("Narx manfiy bo'lmasligi kerak.");
        }

        if (request.ProductCategoryId.HasValue && !await repository.Query<ProductCategory>().AnyAsync(x => x.Id == request.ProductCategoryId && !x.IsDeleted, cancellationToken))
        {
            return ResponseResult.CreateError("Product category topilmadi.", WebErrorConstant.ProductCategoryNotExists);
        }

        foreach (var item in request.ComboItems ?? Array.Empty<ProductComboItemRequest>())
        {
            if (item.Quantity <= 0)
            {
                return ResponseResult.CreateError("Combo item miqdori 0 dan katta bo'lishi kerak.");
            }

            if (!await repository.Query<Product>().AnyAsync(x => x.Id == item.ProductId && !x.IsDeleted, cancellationToken))
            {
                return ResponseResult.CreateError("Combo product topilmadi.", WebErrorConstant.ProductNotExists);
            }
        }

        return ResponseResult.CreateSuccess();
    }

    private static void Apply(Product entity, ProductRequest request)
    {
        entity.Title = request.Title.Trim();
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.SelfPrice = request.SelfPrice;
        entity.IsActive = request.IsActive;
        entity.IsCombo = request.IsCombo;
        entity.UseOwnRecipeForCombo = request.UseOwnRecipeForCombo;
        entity.ProductCategoryId = request.ProductCategoryId;

        foreach (var item in request.ComboItems ?? Array.Empty<ProductComboItemRequest>())
        {
            entity.ComboItems.Add(new ProductComboItem { ProductId = item.ProductId, Quantity = item.Quantity });
        }
    }
}
