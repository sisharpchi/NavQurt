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
        var result = await GetListAsync(new IngredientListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<IngredientDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<IngredientDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<IngredientDto>>> GetListAsync(IngredientListRequest request, CancellationToken cancellationToken = default)
    {
        var query = IngredientQuery().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.Title.ToLower().Contains(search) || x.Unit.ToLower().Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(x => x.IngredientCategoryId == request.CategoryId.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        if (request.OnlyWithoutRecipe)
        {
            query = query.Where(x => x.Recipe == null || x.Recipe.IsDeleted);
        }

        var items = await query
            .OrderBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<IngredientDto>>.CreateSuccess(new ListResponse<IngredientDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<IngredientDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await IngredientQuery().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        return entity == null ? NotFound<IngredientDto>("Ingredient") : ResponseResult<IngredientDto>.CreateSuccess(entity.ToDto());
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

        entity = await IngredientQuery().FirstAsync(x => x.Id == entity.Id, cancellationToken);
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

        entity = await IngredientQuery().FirstAsync(x => x.Id == entity.Id, cancellationToken);
        return ResponseResult<IngredientDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Ingredient>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Ingredient");
        }

        entity.IsDeleted = true;
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
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

        if (request.MinRemainderLimit < 0 || request.AverageSelfPrice < 0)
        {
            return ResponseResult.CreateError("Ingredient limit va self price manfiy bo'lmasligi kerak.");
        }

        if (request.IngredientCategoryId.HasValue && !await repository.Query<IngredientCategory>().AnyAsync(x => x.Id == request.IngredientCategoryId && !x.IsDeleted, cancellationToken))
        {
            return ResponseResult.CreateError("Ingredient category topilmadi.", WebErrorConstant.IngredientCategoryNotExists);
        }

        return ResponseResult.CreateSuccess();
    }

    private IQueryable<Ingredient> IngredientQuery() =>
        repository.Query<Ingredient>()
            .Include(x => x.IngredientCategory)
            .Include(x => x.Recipe);

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
