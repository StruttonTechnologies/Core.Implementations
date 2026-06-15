using MediatR;
using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Queries;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles queries to retrieve a single entity by its ID using IReadService.
/// </summary>
/// <typeparam name="TDto">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public class GetByIdQueryHandler<TDto, TKey>(
    IReadRepository<TDto, TKey> readRepository,
    ILogger<GetByIdQueryHandler<TDto, TKey>> logger) : IRequestHandler<GetByIdQuery<TDto, TKey>, TDto?>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IReadRepository<TDto, TKey> _readRepository = readRepository;
    private readonly ILogger<GetByIdQueryHandler<TDto, TKey>> _logger = logger;

    public async Task<TDto?> Handle(GetByIdQuery<TDto, TKey> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetByIdQuery for entity type {EntityType} with ID {Id}.", typeof(TDto).Name, request.Id);

        var entity = await _readRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
        {
            _logger.LogWarning("Entity of type {EntityType} with ID {Id} not found.", typeof(TDto).Name, request.Id);
        }

        return entity;
    }
}
