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
        var items = await repository.Query<IngredientCategory>(x => !x.IsDeleted)
            .OrderBy(x => x.Title)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<IngredientCategoryDto>>.CreateSuccess(items);
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

        entity.Title = request.Title.Trim();
        entity.IsActive = request.IsActive;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<IngredientCategoryDto>.CreateSuccess(entity.ToDto());
    }
}
