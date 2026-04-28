using Microsoft.EntityFrameworkCore;
using NavQurt.Core.Persistence;

namespace NavQurt.Infrastructure.Persistence;

internal class UnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _context = context;

    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
