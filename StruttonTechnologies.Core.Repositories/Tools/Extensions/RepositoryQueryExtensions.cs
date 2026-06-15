using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace StruttonTechnologies.Core.Infrastructure.Extensions;

public static class RepositoryQueryExtensions
{
    /// <summary>
    /// Applies includes to the query for eager loading.
    /// </summary>
    public static IQueryable<T> ApplyIncludes<T>(
        this IQueryable<T> query,
        params Expression<Func<T, object>>[] includeProperties) where T : class
    {
        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return query;
    }

    /// <summary>
    /// Applies a filter to the query.
    /// </summary>
    public static IQueryable<T> ApplyFilter<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>>? filter) where T : class
    {
        if (filter != null)
        {
            query = query.Where(filter);
        }
        return query;
    }

    /// <summary>
    /// Applies sorting to the query.
    /// </summary>
    public static IQueryable<T> ApplySorting<T, TKey>(
        this IQueryable<T> query,
        Expression<Func<T, TKey>> orderBy,
        bool ascending) where T : class
    {
        return ascending ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
    }

    /// <summary>
    /// Applies pagination to the query.
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        int? pageNumber,
        int? pageSize) where T : class
    {
        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query
                .Skip((pageNumber.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);
        }
        return query;
    }

    /// <summary>
    /// Processes the query in batches.
    /// </summary>
    public static async Task<IEnumerable<T>> ProcessInBatchesAsync<T>(
        this IQueryable<T> query,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        var results = new List<T>();
        int skip = 0;

        while (true)
        {
            var batch = await query
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            if (!batch.Any())
            {
                break;
            }

            results.AddRange(batch);
            skip += batchSize;
        }

        return results;
    }
}
