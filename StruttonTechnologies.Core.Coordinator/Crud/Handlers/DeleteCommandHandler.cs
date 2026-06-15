using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Commands;
using StruttonTechnologies.Core.ToolKit.Exceptions;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles the deletion of an entity using the CQRS pattern.
/// </summary>
/// <typeparam name="TDto">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public class DeleteCommandHandler<TDto, TKey>(
    IDeleteRepository<TDto, TKey> deleteRepository,
    ILogger<DeleteCommandHandler<TDto, TKey>> logger) : IRequestHandler<DeleteCommand<TDto, TKey>, Unit>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IDeleteRepository<TDto, TKey> _deleteRepository = deleteRepository;
    private readonly ILogger<DeleteCommandHandler<TDto, TKey>> _logger = logger;

    public async Task<Unit> Handle(DeleteCommand<TDto, TKey> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DeleteCommand for entity of type {EntityType} with ID {Id}.", typeof(TDto).Name, request.Id);

        TDto? deleted = await _deleteRepository.DeleteAsync(request.Id, cancellationToken);
        if (deleted is null)
        {
            _logger.LogWarning("Delete failed: entity of type {EntityType} with ID {Id} not found.", typeof(TDto).Name, request.Id);
            throw new EntityNotFoundException($"Entity with ID {request.Id} not found.");
        }

        _logger.LogInformation("Entity of type {EntityType} with ID {Id} deleted successfully.", typeof(TDto).Name, request.Id);
        return Unit.Value;
    }
}
