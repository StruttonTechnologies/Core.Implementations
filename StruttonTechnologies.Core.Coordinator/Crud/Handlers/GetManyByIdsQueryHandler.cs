using MediatR;
using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles queries for retrieving multiple DTOs by their IDs using IReadRepository.
/// Supports sorting, pagination, batching, and eager loading.
/// </summary>
/// <typeparam name="TDto">The DTO type.</typeparam>
/// <typeparam name="TKey">The identifier type.</typeparam>
public class GetManyByIdsQueryHandler<TDto, TKey>(
    IReadRepository<TDto, TKey> readRepository,
    ILogger<GetManyByIdsQueryHandler<TDto, TKey>> logger)
    : IRequestHandler<GetManyByIdsQuery<TDto, TKey>, IEnumerable<TDto>>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IReadRepository<TDto, TKey> _readRepository = readRepository;
    private readonly ILogger<GetManyByIdsQueryHandler<TDto, TKey>> _logger = logger;

    public async Task<IEnumerable<TDto>> Handle(
        GetManyByIdsQuery<TDto, TKey> request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetManyByIdsQuery for {EntityType} with {IdCount} IDs.",
            typeof(TDto).Name,
            request.Ids is ICollection<TKey> col ? col.Count : -1);

        var results = await _readRepository.GetManyByIdsAsync(
            ids: request.Ids,
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

        _logger.LogInformation(
            "Retrieved {Count} entities of type {EntityType}.",
            results is ICollection<TDto> resultCol ? resultCol.Count : -1,
            typeof(TDto).Name);

        return results;
    }
}
