using System.Linq.Expressions;

using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Orchestration.Crud;

public class ReadService<TEntity, TKey>(ICrudRepository<TEntity, TKey> repository) : IReadService<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    public Task<TEntity?> GetByIdAsync(
        TKey id,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return repository.GetByIdAsync(id, cancellationToken, includeProperties);
    }

    public Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return repository.GetAllAsync();
    }

    public Task<IEnumerable<TEntity>> GetAllAsync<TSortKey>(
        bool isSorted = false,
        Expression<Func<TEntity, TSortKey>>? orderBy = null,
        bool ascending = true,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        bool isBatched = false,
        int batchSize = 100,
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return repository.GetByFilterAsync(
            filter,
            isSorted,
            orderBy,
            ascending,
            isPaginated,
            pageNumber,
            pageSize,
            isBatched,
            batchSize,
            cancellationToken,
            includeProperties);
    }

    public Task<IEnumerable<TEntity>> GetManyByIdsAsync<TSortKey>(
        IEnumerable<TKey> ids,
        bool isSorted = false,
        Expression<Func<TEntity, TSortKey>>? orderBy = null,
        bool ascending = true,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        bool isBatched = false,
        int batchSize = 100,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return repository.GetManyByIdsAsync(
            ids,
            isSorted,
            orderBy,
            ascending,
            isPaginated,
            pageNumber,
            pageSize,
            isBatched,
            batchSize,
            cancellationToken,
            includeProperties);
    }

    public Task<IEnumerable<TEntity>> GetByFilterAsync<TSortKey>(
        Expression<Func<TEntity, bool>>? filter = null,
        bool isSorted = false,
        Expression<Func<TEntity, TSortKey>>? orderBy = null,
        bool ascending = true,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        bool isBatched = false,
        int batchSize = 100,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return repository.GetByFilterAsync(
            filter,
            isSorted,
            orderBy,
            ascending,
            isPaginated,
            pageNumber,
            pageSize,
            isBatched,
            batchSize,
            cancellationToken,
            includeProperties);
    }

    public Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(filter, cancellationToken);
    }

    public Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return repository.ExistsAsync(filter, cancellationToken);
    }
}
