using System.Linq.Expressions;
using NavQurt.Core.Entities;

namespace NavQurt.Core.Persistence;

public interface IRepository : IDisposable
{
    IUnitOfWork UnitOfWork { get; }

    Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = default)
        where TEntity : class, IEntity<int>;

    Task<decimal> SumAsync<TEntity>(
        Expression<Func<TEntity, decimal>> selector,
        Expression<Func<TEntity, bool>>? predicate = default)
        where TEntity : class, IEntity<int>;

    Task<TEntity?> GetAsync<TEntity>(object? id)
        where TEntity : class;

    Task<TEntity?> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class, IEntity<int>;

    Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>>? predicate = default)
        where TEntity : class, IEntity<int>;

    Task<Dictionary<TKey, TEntity>> GetDictionaryAsync<TKey, TEntity>(
        Func<TEntity, TKey> keySelector,
        Expression<Func<TEntity, bool>>? predicate = default)
        where TEntity : class, IEntity<int>
        where TKey : notnull;

    IQueryable<TEntity> Query<TEntity>(Expression<Func<TEntity, bool>>? predicate = default)
        where TEntity : class;

    Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class, IEntity<int>;

    void Add<TEntity>(TEntity entity)
        where TEntity : class;

    void Update<TEntity>(TEntity entity)
        where TEntity : class, IEntity<int>;

    void Delete<TEntity>(TEntity? entity)
        where TEntity : class;
}
