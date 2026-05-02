using Microsoft.EntityFrameworkCore;
using NavQurt.Application.Common;
using NavQurt.Application.Contracts;
using NavQurt.Core.Entities.Business;
using NavQurt.Core.Persistence;
using NavQurt.Shared;

namespace NavQurt.Application.Services;

internal sealed class WorkerService(IMainRepository repository) : BusinessServiceBase, IWorkerService
{
    public async Task<ResponseResult<IReadOnlyCollection<WorkerDto>>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var result = await GetListAsync(new WorkerListRequest(), cancellationToken);
        return result.Success
            ? ResponseResult<IReadOnlyCollection<WorkerDto>>.CreateSuccess(result.Value!.Items)
            : ResponseResult<IReadOnlyCollection<WorkerDto>>.CreateError(result.Error!, result.ErrorCode);
    }

    public async Task<ResponseResult<ListResponse<WorkerDto>>> GetListAsync(WorkerListRequest request, CancellationToken cancellationToken = default)
    {
        var query = repository.Query<Worker>(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(x => x.FullName.ToLower().Contains(search) || (x.PhoneNumber != null && x.PhoneNumber.ToLower().Contains(search)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var items = await query
            .OrderBy(x => x.FullName)
            .ToListAsync(cancellationToken);

        return ResponseResult<ListResponse<WorkerDto>>.CreateSuccess(new ListResponse<WorkerDto>(items.Select(x => x.ToDto()).ToList(), items.Count));
    }

    public async Task<ResponseResult<WorkerDto>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Worker>(x => x.Id == id && !x.IsDeleted);
        return entity == null ? NotFound<WorkerDto>("Worker") : ResponseResult<WorkerDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<WorkerDto>> CreateAsync(WorkerRequest request, CancellationToken cancellationToken = default)
    {
        if (!HasText(request.FullName))
        {
            return BadRequest<WorkerDto>("Worker FIO majburiy.");
        }

        var entity = new Worker { FullName = request.FullName.Trim(), PhoneNumber = request.PhoneNumber, IsActive = request.IsActive };
        await repository.AddAsync(entity);
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<WorkerDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult<WorkerDto>> UpdateAsync(int id, WorkerRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Worker>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound<WorkerDto>("Worker");
        }

        if (!HasText(request.FullName))
        {
            return BadRequest<WorkerDto>("Worker FIO majburiy.");
        }

        entity.FullName = request.FullName.Trim();
        entity.PhoneNumber = request.PhoneNumber;
        entity.IsActive = request.IsActive;

        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult<WorkerDto>.CreateSuccess(entity.ToDto());
    }

    public async Task<ResponseResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await repository.GetAsync<Worker>(x => x.Id == id && !x.IsDeleted);
        if (entity == null)
        {
            return NotFound("Worker");
        }

        entity.IsDeleted = true;
        await repository.UnitOfWork.CommitAsync(cancellationToken);
        return ResponseResult.CreateSuccess();
    }
}
