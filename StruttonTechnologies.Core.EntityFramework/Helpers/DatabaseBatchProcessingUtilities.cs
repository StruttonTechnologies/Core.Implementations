using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace StruttonTechnologies.Core.EntityFramework.Helpers;

public static class DatabaseBatchProcessor<T>
{
    // Process all records without any ID filtering
    public static async Task<IEnumerable<T>> ProcessInBatchesAsync(
        IQueryable<T> query,
        int batchSize,
        Func<IQueryable<T>, Task<IEnumerable<T>>> processBatch,
        CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));
        if (processBatch == null) throw new ArgumentNullException(nameof(processBatch));

        var results = new List<T>();
        var totalItems = await query.CountAsync(cancellationToken);
        var totalBatches = (int)Math.Ceiling((double)totalItems / batchSize);

        for (int i = 0; i < totalBatches; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batchQuery = query.Skip(i * batchSize).Take(batchSize);
            var batchResults = await processBatch(batchQuery);
            results.AddRange(batchResults);
        }

        return results;
    }

    // Process by a list of IDs (without includeProperties)
    public static async Task<IEnumerable<TEntity>> ProcessInBatchesAsync<TKey, TEntity>(
        IEnumerable<TKey> keys,
        DbSet<TEntity> dbSet,
        string keyPropertyName = "Id",
        int batchSize = 100,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (dbSet == null) throw new ArgumentNullException(nameof(dbSet));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        var results = new List<TEntity>();
        var keyList = keys.ToList();

        for (int i = 0; i < keyList.Count; i += batchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = keyList.Skip(i).Take(batchSize).ToList();
            var query = dbSet.Where(BuildContainsExpression<TEntity, TKey>(keyPropertyName, batch));
            var batchResults = await query.ToListAsync(cancellationToken);
            results.AddRange(batchResults);
        }

        return results;
    }

    // Process by a list of IDs with includeProperties
    public static async Task<IEnumerable<TEntity>> ProcessInBatchesAsync<TKey, TEntity>(
        IEnumerable<TKey> keys,
        DbSet<TEntity> dbSet,
        string keyPropertyName = "Id",
        int batchSize = 100,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        if (keys == null) throw new ArgumentNullException(nameof(keys));
        if (dbSet == null) throw new ArgumentNullException(nameof(dbSet));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        var results = new List<TEntity>();
        var keyList = keys.ToList();

        for (int i = 0; i < keyList.Count; i += batchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = keyList.Skip(i).Take(batchSize).ToList();
            IQueryable<TEntity> query = dbSet;

            // Apply includes if any
            if (includeProperties != null)
            {
                foreach (var include in includeProperties)
                {
                    query = query.Include(include);
                }
            }

            var predicate = BuildContainsExpression<TEntity, TKey>(keyPropertyName, batch);
            var batchResults = await query.Where(predicate).ToListAsync(cancellationToken);
            results.AddRange(batchResults);
        }

        return results;
    }

    // Process all records with includeProperties
    public static async Task<IEnumerable<TEntity>> ProcessInBatchesAsync<TEntity>(
        DbSet<TEntity> dbSet,
        int batchSize = 100,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        if (dbSet == null) throw new ArgumentNullException(nameof(dbSet));
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        var results = new List<TEntity>();
        IQueryable<TEntity> query = dbSet;

        // Apply includes if any
        if (includeProperties != null)
        {
            foreach (var include in includeProperties)
            {
                query = query.Include(include);
            }
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalBatches = (int)Math.Ceiling((double)totalItems / batchSize);

        for (int i = 0; i < totalBatches; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batchQuery = query.Skip(i * batchSize).Take(batchSize);
            var batchResults = await batchQuery.ToListAsync(cancellationToken);
            results.AddRange(batchResults);
        }

        return results;
    }

    // Helper method to build the Contains expression
    private static Expression<Func<TEntity, bool>> BuildContainsExpression<TEntity, TKey>(
        string propertyName,
        IEnumerable<TKey> values)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var property = Expression.Property(parameter, propertyName);
        var constant = Expression.Constant(values);

        var containsMethod = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(TKey));

        var body = Expression.Call(containsMethod, constant, property);
        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}
