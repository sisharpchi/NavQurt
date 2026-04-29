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
        var items = await repository.Query<Worker>(x => !x.IsDeleted)
            .OrderBy(x => x.FullName)
            .Select(x => x.ToDto())
            .ToListAsync(cancellationToken);

        return ResponseResult<IReadOnlyCollection<WorkerDto>>.CreateSuccess(items);
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
