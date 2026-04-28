using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class IngredientService(IMainRepository repository) : BusinessServiceBase, IIngredientService
{
    public async Task<ResponseResult<IReadOnlyCollection<IngredientDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var items = await repository.Query<Ingredient>(x => !x.IsDeleted)
            .OrderBy(x => x.Title)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<IngredientDto>>.CreateSuccess(items);
    }

    public async Task<ResponseResult<IngredientDto>> CreateAsync(IngredientRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<IngredientDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        var entity = new Ingredient();
        Apply(entity, request);
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return ResponseResult<IngredientDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<IngredientDto>> UpdateAsync(int id, IngredientRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Ingredient>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<IngredientDto>("Ingredient");
        }

        var validation = await ValidateAsync(request, cancellationToken);
        if (!validation.Success)
        {
            return ResponseResult<IngredientDto>.CreateError(validation.Error!, validation.ErrorCode);
        }

        Apply(entity, request);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return ResponseResult<IngredientDto>.CreateSuccess(entity.ToDto());
    }

    private async Task<ResponseResult> ValidateAsync(IngredientRequest request, CancellationToken cancellationToken)
    {
        if (!HasText(request.Title))
        {
            return ResponseResult.CreateError("Ingredient nomi majburiy.");
        }

        if (!HasText(request.Unit))
        {
            return ResponseResult.CreateError("Ingredient unit majburiy.");
        }

        if (request.IngredientCategoryId.HasValue && !await repository.Query<IngredientCategory>().AnyAsync(x => x.Id == request.IngredientCategoryId && !x.IsDeleted, cancellationToken))
        {
            return ResponseResult.CreateError("Ingredient category topilmadi.", WebErrorConstant.IngredientCategoryNotExists);
        }

        return ResponseResult.CreateSuccess();
    }

    private static void Apply(Ingredient entity, IngredientRequest request)
    {
        entity.Title = request.Title.Trim();
        entity.Unit = request.Unit.Trim();
        entity.MinRemainderLimit = request.MinRemainderLimit;
        entity.AverageSelfPrice = request.AverageSelfPrice;
        entity.IsActive = request.IsActive;
        entity.IngredientCategoryId = request.IngredientCategoryId;
    }
}
