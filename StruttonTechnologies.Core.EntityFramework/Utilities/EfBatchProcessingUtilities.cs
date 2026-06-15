using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace StruttonTechnologies.Core.EntityFramework.Utilities;

/// <summary>
/// Provides helper methods for retrieving and processing Entity Framework query results in batches.
/// </summary>
/// <remarks>
/// These utilities are intended for scenarios where loading a large result set or
/// filtering by a large collection of keys would otherwise result in inefficient or
/// oversized queries.
/// </remarks>
public static class EfBatchProcessingUtilities
{
    /// <summary>
    /// Retrieves the results of an ordered query in sequential batches and returns the
    /// combined results as a single list.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="query">The ordered query to process.</param>
    /// <param name="batchSize">The number of records to retrieve per batch.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A list containing all records returned by the query.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="query"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="batchSize"/> is less than 1.
    /// </exception>
    /// <remarks>
    /// The supplied query should have a stable ordering applied before batching.
    /// Without deterministic ordering, paging with <c>Skip</c> and <c>Take</c>
    /// may produce inconsistent results.
    /// </remarks>
    public static async Task<List<TEntity>> ProcessInBatchesAsync<TEntity>(
        IQueryable<TEntity> query,
        int batchSize,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(query);

        if (batchSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalBatches = (int)Math.Ceiling((double)totalItems / batchSize);
        var results = new List<TEntity>(totalItems);

        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batchResults = await query
                .Skip(batchIndex * batchSize)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            results.AddRange(batchResults);
        }

        return results;
    }

    /// <summary>
    /// Retrieves all records from a <see cref="DbSet{TEntity}"/> in sequential batches
    /// and returns the combined results as a single list.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="dbSet">The entity set to process.</param>
    /// <param name="batchSize">The number of records to retrieve per batch.</param>
    /// <param name="includeProperties">Optional navigation properties to include.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A list containing all records returned by the query.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="dbSet"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="batchSize"/> is less than 1.
    /// </exception>
    /// <remarks>
    /// The underlying query should have a stable ordering applied before batching if
    /// deterministic paging is required.
    /// </remarks>
    public static Task<List<TEntity>> ProcessInBatchesAsync<TEntity>(
        DbSet<TEntity> dbSet,
        int batchSize = 100,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(dbSet);

        if (batchSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        IQueryable<TEntity> query = dbSet;

        if (includeProperties is not null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return ProcessInBatchesAsync(query, batchSize, cancellationToken);
    }

    /// <summary>
    /// Processes an ordered query one batch at a time without accumulating all results
    /// in memory.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <param name="query">The ordered query to process.</param>
    /// <param name="batchSize">The number of records to retrieve per batch.</param>
    /// <param name="batchProcessor">
    /// The asynchronous callback that processes each retrieved batch.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous batch processing operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="query"/> or <paramref name="batchProcessor"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="batchSize"/> is less than 1.
    /// </exception>
    /// <remarks>
    /// The supplied query should have a stable ordering applied before batching.
    /// Without deterministic ordering, paging with <c>Skip</c> and <c>Take</c>
    /// may produce inconsistent results.
    /// </remarks>
    public static async Task ProcessEachBatchAsync<TEntity>(
        IQueryable<TEntity> query,
        int batchSize,
        Func<List<TEntity>, Task> batchProcessor,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(batchProcessor);

        if (batchSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var totalBatches = (int)Math.Ceiling((double)totalItems / batchSize);

        for (int batchIndex = 0; batchIndex < totalBatches; batchIndex++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batchResults = await query
                .Skip(batchIndex * batchSize)
                .Take(batchSize)
                .ToListAsync(cancellationToken);

            await batchProcessor(batchResults);
        }
    }

    /// <summary>
    /// Retrieves entities by key in smaller batches to avoid oversized or inefficient
    /// SQL <c>IN</c> clauses.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="keys">The keys to retrieve.</param>
    /// <param name="dbSet">The entity set to query.</param>
    /// <param name="keySelector">The expression used to select the entity key.</param>
    /// <param name="batchSize">The number of keys to process per batch.</param>
    /// <param name="includeProperties">Optional navigation properties to include.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A list containing all matching entities.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="keys"/>, <paramref name="dbSet"/>, or
    /// <paramref name="keySelector"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="batchSize"/> is less than 1.
    /// </exception>
    /// <remarks>
    /// This method is intended for scenarios where a large key set would otherwise
    /// produce an inefficient or oversized SQL <c>IN</c> clause. Keys are split into
    /// smaller batches to improve query performance and reduce provider limitations.
    /// </remarks>
    public static async Task<List<TEntity>> ProcessByKeysInBatchesAsync<TEntity, TKey>(
        IEnumerable<TKey> keys,
        DbSet<TEntity> dbSet,
        Expression<Func<TEntity, TKey>> keySelector,
        int batchSize = 100,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(keySelector);

        if (batchSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        var keyList = keys.Distinct().ToList();
        var results = new List<TEntity>();

        for (int index = 0; index < keyList.Count; index += batchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentBatch = keyList.Skip(index).Take(batchSize).ToList();
            IQueryable<TEntity> query = dbSet;

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var predicate = BuildContainsExpression(keySelector, currentBatch);
            var batchResults = await query
                .Where(predicate)
                .ToListAsync(cancellationToken);

            results.AddRange(batchResults);
        }

        return results;
    }

    /// <summary>
    /// Processes entities matched by a large key set in smaller batches without
    /// accumulating all results in memory.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="keys">The keys to retrieve.</param>
    /// <param name="dbSet">The entity set to query.</param>
    /// <param name="keySelector">The expression used to select the entity key.</param>
    /// <param name="batchProcessor">
    /// The asynchronous callback that processes each retrieved batch.
    /// </param>
    /// <param name="batchSize">The number of keys to process per batch.</param>
    /// <param name="includeProperties">Optional navigation properties to include.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous batch processing operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="keys"/>, <paramref name="dbSet"/>,
    /// <paramref name="keySelector"/>, or <paramref name="batchProcessor"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="batchSize"/> is less than 1.
    /// </exception>
    /// <remarks>
    /// This method is intended for scenarios where a large key set would otherwise
    /// produce an inefficient or oversized SQL <c>IN</c> clause. Keys are split into
    /// smaller batches, and each batch is processed independently.
    /// </remarks>
    public static async Task ProcessEachKeyBatchAsync<TEntity, TKey>(
        IEnumerable<TKey> keys,
        DbSet<TEntity> dbSet,
        Expression<Func<TEntity, TKey>> keySelector,
        Func<List<TEntity>, Task> batchProcessor,
        int batchSize = 100,
        Expression<Func<TEntity, object>>[]? includeProperties = null,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(keys);
        ArgumentNullException.ThrowIfNull(dbSet);
        ArgumentNullException.ThrowIfNull(keySelector);
        ArgumentNullException.ThrowIfNull(batchProcessor);

        if (batchSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(batchSize));
        }

        var keyList = keys.Distinct().ToList();

        for (int index = 0; index < keyList.Count; index += batchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var currentBatch = keyList.Skip(index).Take(batchSize).ToList();
            IQueryable<TEntity> query = dbSet;

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            var predicate = BuildContainsExpression(keySelector, currentBatch);
            var batchResults = await query
                .Where(predicate)
                .ToListAsync(cancellationToken);

            await batchProcessor(batchResults);
        }
    }

    /// <summary>
    /// Builds a predicate that filters entities whose selected key is contained in the
    /// provided values.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being queried.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="keySelector">The expression used to select the entity key.</param>
    /// <param name="values">The values to match.</param>
    /// <returns>
    /// A predicate suitable for use in a LINQ <c>Where</c> clause.
    /// </returns>
    private static Expression<Func<TEntity, bool>> BuildContainsExpression<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> keySelector,
        IEnumerable<TKey> values)
    {
        var valueList = values.ToList();
        var parameter = keySelector.Parameters[0];

        var body = Expression.Call(
            typeof(Enumerable),
            nameof(Enumerable.Contains),
            new[] { typeof(TKey) },
            Expression.Constant(valueList),
            keySelector.Body);

        return Expression.Lambda<Func<TEntity, bool>>(body, parameter);
    }
}