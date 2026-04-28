using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NavQurt.Core.Persistence;

public interface IMainRepository : IRepository
{
    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    DatabaseFacade Database { get; }
}
