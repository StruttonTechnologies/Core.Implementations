using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles queries for retrieving DTOs by filter using IReadRepository.
/// Supports sorting, pagination, batching, and eager loading.
/// </summary>
/// <typeparam name="TDto">The DTO type.</typeparam>
/// <typeparam name="TKey">The type of the key to sort by.</typeparam>
public class GetByFilterQueryHandler<TDto, TKey>(
    IReadRepository<TDto, TKey> readRepository,
    ILogger<GetByFilterQueryHandler<TDto, TKey>> logger)
    : IRequestHandler<GetByFilterQuery<TDto, TKey>, IEnumerable<TDto>>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IReadRepository<TDto, TKey> _readRepository = readRepository;
    private readonly ILogger<GetByFilterQueryHandler<TDto, TKey>> _logger = logger;

    public async Task<IEnumerable<TDto>> Handle(
        GetByFilterQuery<TDto, TKey> request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetByFilterQuery for {EntityType}.", typeof(TDto).Name);

        IEnumerable<TDto> results = await _readRepository.GetByFilterAsync(
            filter: request.Filter,
            isSorted: request.IsSorted,
            orderBy: request.OrderBy,
            ascending: request.Ascending,
            isPaginated: request.IsPaginated,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            isBatched: request.IsBatched,
            batchSize: request.BatchSize,
            cancellationToken: cancellationToken,
            includeProperties: request.IncludeProperties ?? []
        );

        _logger.LogInformation("Retrieved {Count} entities of type {EntityType}.",
            results is ICollection<TDto> col ? col.Count : -1,
            typeof(TDto).Name);

        return results;
    }
}
