using StruttonTechnologies.Core.Coordinator.Contracts.Crud.Commands;
using StruttonTechnologies.Core.Orchestration.Contracts.Crud;
using StruttonTechnologies.Core.Orchestration.Crud;

namespace StruttonTechnologies.Core.Coordinator.Crud.Handlers;

/// <summary>
/// Handles the creation of entities using the CQRS pattern.
/// </summary>
/// <typeparam name="TDto">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity's key.</typeparam>
public class CreateCommandHandler<TDto, TKey>(
    CreateService<TDto, TKey> createService,
    ILogger<CreateCommandHandler<TDto, TKey>> logger) : IRequestHandler<CreateCommand<TDto, TKey>, TDto>
    where TDto : class
    where TKey : IEquatable<TKey>
{
    private readonly ICreateService<TDto, TKey> _createService = createService;
    private readonly ILogger<CreateCommandHandler<TDto, TKey>> _logger = logger;

    public async Task<TDto> Handle(CreateCommand<TDto, TKey> request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCommand for entity of type {EntityType}.", typeof(TDto).Name);

        TDto result = await _createService.CreateAsync(request.Payload, cancellationToken);

        _logger.LogInformation("Entity of type {EntityType} created successfully.", typeof(TDto).Name);
        return result;
    }
}
