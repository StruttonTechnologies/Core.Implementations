using MediatR;
using Microsoft.Extensions.Logging;
using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Commands;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles update commands for entities in the CQRS pattern using IUpdateService.
/// </summary>
/// <typeparam name="TDto">The type of the entity to update.</typeparam>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public class UpdateCommandHandler<TDto, TKey>(
    IUpdateService<TDto, TKey> updateService,
    ILogger<UpdateCommandHandler<TDto, TKey>> logger) : IRequestHandler<UpdateCommand<TDto, TKey>, TDto>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly IUpdateService<TDto, TKey> _updateService = updateService;
    private readonly ILogger<UpdateCommandHandler<TDto, TKey>> _logger = logger;

    public async Task<TDto> Handle(UpdateCommand<TDto, TKey> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCommand for entity of type {EntityType}.", typeof(TDto).Name);

        var updated = await _updateService.UpdateAsync(request.Payload, cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} updated successfully.", typeof(TDto).Name);
        return updated;
    }
}
