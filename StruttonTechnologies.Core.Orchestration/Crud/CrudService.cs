using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace StruttonTechnologies.Core.Orchestration.Crud;

/// <summary>
/// Unified CRUD service that delegates to Create, Read, Update, Delete, and SoftDelete services.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
/// <typeparam name="TKey">The entity key type.</typeparam>
public class CrudService<TEntity, TKey>(
    ICrudRepository<TEntity, TKey> repository,
    ILogger<DeleteService<TEntity, TKey>> deleteLogger,
    ILogger<SoftDeleteService<TEntity, TKey>> softDeleteLogger)
    : ICrudService<TEntity, TKey>,
      ICreateService<TEntity,TKey>,
      IReadService<TEntity, TKey>,
      IUpdateService<TEntity, TKey>,
      IDeleteService<TEntity, TKey>,
      ISoftDeleteService<TEntity, TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    // Composition: expose sub-services as properties
    public CreateService<TEntity,TKey> Create { get; } = new(repository);
    public ReadService<TEntity, TKey> Read { get; } = new(repository);
    public UpdateService<TEntity, TKey> Update { get; } = new(repository);
    public DeleteService<TEntity, TKey> Delete { get; } = new(repository, deleteLogger);
    public SoftDeleteService<TEntity, TKey> SoftDelete { get; } = new(repository, softDeleteLogger);

    // Interface forwarding

    // Create
    Task<TEntity> ICreateService<TEntity,TKey>.CreateAsync(TEntity entity, CancellationToken cancellationToken) =>
        Create.CreateAsync(entity, cancellationToken);

    Task<IEnumerable<TEntity>> ICreateService<TEntity,TKey>.BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        Create.BulkInsertAsync(entities, cancellationToken);

    // Read
    Task<TEntity?> IReadService<TEntity, TKey>.GetByIdAsync(
    TKey id,
    CancellationToken cancellationToken,
    params Expression<Func<TEntity, object>>[] includeProperties) =>
        Read.GetByIdAsync(id, cancellationToken, includeProperties);

    Task<IEnumerable<TEntity>> IReadService<TEntity, TKey>.GetAllAsync() =>
        Read.GetAllAsync();

    Task<IEnumerable<TEntity>> IReadService<TEntity, TKey>.GetAllAsync<TSortKey>(
        bool isSorted,
        Expression<Func<TEntity, TSortKey>>? orderBy,
        bool ascending,
        bool isPaginated,
        int pageNumber,
        int pageSize,
        bool isBatched,
        int batchSize,
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken,
        params Expression<Func<TEntity, object>>[] includeProperties) =>
        Read.GetAllAsync<TSortKey>(
            isSorted,
            orderBy,
            ascending,
            isPaginated,
            pageNumber,
            pageSize,
            isBatched,
            batchSize,
            filter,
            cancellationToken,
            includeProperties);

    Task<IEnumerable<TEntity>> IReadService<TEntity, TKey>.GetManyByIdsAsync<TSortKey>(
        IEnumerable<TKey> ids,
        bool isSorted,
        Expression<Func<TEntity, TSortKey>>? orderBy,
        bool ascending,
        bool isPaginated,
        int pageNumber,
        int pageSize,
        bool isBatched,
        int batchSize,
        CancellationToken cancellationToken,
        params Expression<Func<TEntity, object>>[] includeProperties) =>
        Read.GetManyByIdsAsync<TSortKey>(
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

    Task<IEnumerable<TEntity>> IReadService<TEntity, TKey>.GetByFilterAsync<TSortKey>(
        Expression<Func<TEntity, bool>>? filter,
        bool isSorted,
        Expression<Func<TEntity, TSortKey>>? orderBy,
        bool ascending,
        bool isPaginated,
        int pageNumber,
        int pageSize,
        bool isBatched,
        int batchSize,
        CancellationToken cancellationToken,
        params Expression<Func<TEntity, object>>[] includeProperties) =>
        Read.GetByFilterAsync<TSortKey>(
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

    Task<int> IReadService<TEntity, TKey>.CountAsync(
        Expression<Func<TEntity, bool>>? filter,
        CancellationToken cancellationToken) =>
        Read.CountAsync(filter, cancellationToken);

    // Update 
    Task<TEntity> IUpdateService<TEntity, TKey>.UpdateAsync(TEntity entity, CancellationToken cancellationToken) =>
        Update.UpdateAsync(entity, cancellationToken);

    Task<IEnumerable<TEntity>> IUpdateService<TEntity, TKey>.BulkUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken) =>
        Update.BulkUpdateAsync(entities, cancellationToken);

    // Delete
    Task<TEntity?> IDeleteService<TEntity, TKey>.DeleteAsync(TKey id, CancellationToken cancellationToken) =>
        Delete.DeleteAsync(id, cancellationToken);

    Task<IEnumerable<TEntity>> IDeleteService<TEntity, TKey>.BulkDeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken) =>
        Delete.BulkDeleteAsync(ids, cancellationToken);



    Task<TEntity?> ISoftDeleteService<TEntity, TKey>.SoftDeleteAsync(TKey id, CancellationToken cancellationToken) =>
        SoftDelete.SoftDeleteAsync(id, cancellationToken);

    Task<IEnumerable<TEntity>> ISoftDeleteService<TEntity, TKey>.BulkSoftDeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken) =>
        SoftDelete.BulkSoftDeleteAsync(ids, cancellationToken);

}
