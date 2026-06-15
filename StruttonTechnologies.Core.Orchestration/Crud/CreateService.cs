using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Orchestration.Crud;

public class CreateService<TEntity,TKey>(ICrudRepository<TEntity,TKey> repository) : ICreateService<TEntity,TKey>
    where TEntity : class
    where TKey : IEquatable<TKey>
{
    public Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        repository.AddAsync(entity, cancellationToken);

    public Task<IEnumerable<TEntity>> BulkInsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) =>
        repository.BulkInsertAsync(entities, cancellationToken);
}
