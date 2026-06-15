using MediatR;
using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Orchestration.Contracts;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;

/// <summary>
/// Handles EmailExistsQuery requests by delegating to the orchestration layer.
/// </summary>
public class EmailExistsHandler<TKey>
    : IRequestHandler<EmailExistsQuery, bool>
    where TKey : IEquatable<TKey>
{
    private readonly IPersonOrchestration<TKey> _service;

    public EmailExistsHandler(IPersonOrchestration<TKey> service)
    {
        _service = service;
    }

    public Task<bool> Handle(EmailExistsQuery request, CancellationToken cancellationToken) =>
        _service.EmailExistsAsync(request.Email, cancellationToken);
}
