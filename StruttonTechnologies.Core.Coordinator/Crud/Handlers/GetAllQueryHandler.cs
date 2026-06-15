using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles queries to retrieve all entities of a given type using IQueryService.
/// Supports sorting, filtering, pagination, and batching.
/// </summary>
/// <typeparam name="TDto">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public class GetAllQueryHandler<TDto, TKey>(
    IReadRepository<TDto, TKey> repository,
    ILogger<GetAllQueryHandler<TDto, TKey>> logger) : IRequestHandler<GetAllQuery<TDto>, IEnumerable<TDto>>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IReadRepository<TDto, TKey> _repository = repository;
    private readonly ILogger<GetAllQueryHandler<TDto, TKey>> _logger = logger;

    public async Task<IEnumerable<TDto>> Handle(GetAllQuery<TDto> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllQuery for entity type {EntityType}.", typeof(TDto).Name);

        IEnumerable<TDto> results = await _repository.GetAllAsync(
            isSorted: request.IsSorted,
            orderBy: request.OrderBy,
            ascending: request.Ascending,
            isPaginated: request.IsPaginated,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            isBatched: request.IsBatched,
            batchSize: request.BatchSize,
            filter: request.Filter,
            cancellationToken: cancellationToken
        );

        _logger.LogInformation("Retrieved {Count} entities of type {EntityType}.", results is ICollection<TDto> col ? col.Count : -1, typeof(TDto).Name);
        return results;
    }
}
