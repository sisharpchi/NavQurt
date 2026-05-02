using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class IngredientCategoryService(IMainRepository repository) : BusinessServiceBase, IIngredientCategoryService
{
    public async Task<ResponseResult<IReadOnlyCollection<IngredientCategoryDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new IngredientCategoryListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<IngredientCategoryDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<IngredientCategoryDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<IngredientCategoryDto>>> GetListAsync(IngredientCategoryListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<IngredientCategory>(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var items = await query
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<IngredientCategoryDto>>.CreateSuccess(new ListResponse<IngredientCategoryDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<IngredientCategoryDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<IngredientCategory>(x => x.Id == id && !x.IsDeleted);
        return entity == null ? NotFound<IngredientCategoryDto>("Ingredient category") : ResponseResult<IngredientCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<IngredientCategoryDto>> CreateAsync(IngredientCategoryRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasText(request.Title))
        {
            return BadRequest<IngredientCategoryDto>("Ingredient category nomi majburiy.");
        }

        var entity = new IngredientCategory { Title = request.Title.Trim(), IsActive = request.IsActive };
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return ResponseResult<IngredientCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<IngredientCategoryDto>> UpdateAsync(int id, IngredientCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<IngredientCategory>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<IngredientCategoryDto>("Ingredient category");
        }

        if (!HasText(request.Title))
        {
            return BadRequest<IngredientCategoryDto>("Ingredient category nomi majburiy.");
        }

        entity.Title = request.Title.Trim();
        entity.IsActive = request.IsActive;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<IngredientCategoryDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<IngredientCategory>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Ingredient category");
        }

        entity.IsDeleted = true;
        var ingredients = await repository.Query<Ingredient>().Where(x => x.IngredientCategoryId == id && !x.IsDeleted).ToListAsync(cancellationToken);
        foreach (var ingredient in ingredients)
        {
            ingredient.IngredientCategoryId = null;
        }

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
