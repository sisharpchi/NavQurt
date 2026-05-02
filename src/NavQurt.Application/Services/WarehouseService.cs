using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class WarehouseService(IMainRepository repository) : BusinessServiceBase, IWarehouseService
{
    public async Task<ResponseResult<IReadOnlyCollection<WarehouseDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new WarehouseListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<WarehouseDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<WarehouseDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<WarehouseDto>>> GetListAsync(WarehouseListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<Warehouse>(x => !x.IsDeleted);

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
            .OrderByDescending(x => x.IsMain)
            .ThenBy(x => x.Title)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<WarehouseDto>>.CreateSuccess(new ListResponse<WarehouseDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<WarehouseDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Warehouse>(x => x.Id == id && !x.IsDeleted);
        return entity == null ? NotFound<WarehouseDto>("Warehouse") : ResponseResult<WarehouseDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<WarehouseDto>> CreateAsync(WarehouseRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasText(request.Title))
        {
            return BadRequest<WarehouseDto>("Warehouse nomi majburiy.");
        }

        var entity = new Warehouse { Title = request.Title.Trim(), IsActive = request.IsActive };
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);

        return ResponseResult<WarehouseDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<WarehouseDto>> UpdateAsync(int id, WarehouseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Warehouse>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<WarehouseDto>("Warehouse");
        }

        if (!HasText(request.Title))
        {
            return BadRequest<WarehouseDto>("Warehouse nomi majburiy.");
        }

        entity.Title = request.Title.Trim();
        entity.IsActive = request.IsActive;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<WarehouseDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Warehouse>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Warehouse");
        }

        entity.IsDeleted = true;
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
