using NavQurt.Shared;

namespace NavQurt.Application.Contracts;

public interface IWorkerService
{
    Task<ResponseResult<IReadOnlyCollection<WorkerDto>>> GetListAsync(CancellationToken cancellationToken = default);
    Task<ResponseResult<WorkerDto>> CreateAsync(WorkerRequest request, CancellationToken cancellationToken = default);
    Task<ResponseResult<WorkerDto>> UpdateAsync(int id, WorkerRequest request, CancellationToken cancellationToken = default);
}
