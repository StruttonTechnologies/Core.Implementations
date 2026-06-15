using Microsoft.EntityFrameworkCore;
using StruttonTechnologies.Core.Domain.Contracts;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;
using System.Linq.Expressions;

namespace StruttonTechnologies.Core.Repositories.Crud;

public partial class CrudRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Retrieves multiple entities by their IDs with optional pagination and eager loading.
    /// Non-generic overload delegates to the generic version with TSortKey = object.
    /// </summary>
    public Task<IEnumerable<TEntity>> GetManyByIdsAsync(
        IEnumerable<TKey> ids,
        bool isPaginated = false,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return GetManyByIdsAsync<object>(
            ids,
            isSorted: false,
            orderBy: null,
            ascending: true,
            isPaginated,
            pageNumber,
            pageSize,
            isBatched: false,
            batchSize: 100,
            cancellationToken,
            includeProperties);
    }

    /// <summary>
    /// Retrieves multiple entities by their IDs with optional sorting, pagination, and eager loading.
    /// </summary>
    /// <typeparam name="TSortKey">The type of the property used for sorting.</typeparam>
    public async Task<IEnumerable<TEntity>> GetManyByIdsAsync<TSortKey>(
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
        if (ids == null || !ids.Any())
        {
            throw new ArgumentException("The list of IDs cannot be null or empty.", nameof(ids));
        }

        return await ExecuteQueryWithExceptionHandlingAsync(async () =>
        {
            IQueryable<TEntity> query = DbSet.Where(e => ids.Contains(EF.Property<TKey>(e, "Id")));

            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            if (isSorted && orderBy != null)
            {
                query = ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            }

            if (isPaginated)
            {
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }

            return await query.ToListAsync(cancellationToken);
        });
    }
}

