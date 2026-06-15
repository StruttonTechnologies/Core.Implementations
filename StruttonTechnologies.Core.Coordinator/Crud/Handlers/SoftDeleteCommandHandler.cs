using MediatR;

using Microsoft.Extensions.Logging;

using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Commands;
using StruttonTechnologies.Core.Repositories.Contracts.Crud;
using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles the soft delete command for a given entity type using ISoftDeleteService.
/// </summary>
/// <typeparam name="TDto">The type of the entity to be soft deleted.</typeparam>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public class SoftDeleteCommandHandler<TDto, TKey>(
    ISoftDeleteRepository<TDto, TKey> softDeleteRepository,
    ILogger<SoftDeleteCommandHandler<TDto, TKey>> logger) : IRequestHandler<SoftDeleteCommand<TDto, TKey>, Unit>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly ISoftDeleteRepository<TDto, TKey> _softDeleteRepository = softDeleteRepository;
    private readonly ILogger<SoftDeleteCommandHandler<TDto, TKey>> _logger = logger;

    public async Task<Unit> Handle(SoftDeleteCommand<TDto, TKey> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling SoftDeleteCommand for entity of type {EntityType} with ID {Id}.", typeof(TDto).Name, request.Id);

        TDto? result = await _softDeleteRepository.SoftDeleteAsync(request.Id, cancellationToken);
        if (result is null)
        {
            _logger.LogWarning("Soft delete failed: entity of type {EntityType} with ID {Id} not found.", typeof(TDto).Name, request.Id);
            throw new EntityNotFoundException($"Entity with ID {request.Id} not found.");
        }

        _logger.LogInformation("Entity of type {EntityType} with ID {Id} soft deleted successfully.", typeof(TDto).Name, request.Id);
        return Unit.Value;
    }
}
