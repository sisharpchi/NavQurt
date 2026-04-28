using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NavQurt.Core.Persistence;
using NavQurt.Infrastructure.Data;

namespace NavQurt.Infrastructure.Persistence;

internal class MainRepository(MainDbContext context, UnitOfWork<MainDbContext> unitOfWork) : GenericRepository<MainDbContext>(context, unitOfWork), IMainRepository
{
    public DatabaseFacade Database => Context.Database;

    public DbSet<TEntity> Set<TEntity>()
        where TEntity : class
    {
        return Context.Set<TEntity>();
    }
}
