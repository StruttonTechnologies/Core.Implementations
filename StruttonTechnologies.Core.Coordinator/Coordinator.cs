using StruttonTechnologies.Core.Coordinator.Contracts;

namespace StruttonTechnologies.Core.Coordinator;

/// <summary>
/// Concrete implementation of ICoordinator using MediatR.
/// </summary>
public class Coordinator : ICoordinator
{
    private readonly IMediator _mediator;

    public Coordinator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        return _mediator.Send(request, cancellationToken);
    }

    public async Task Send(
        IRequest<Unit> request,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(request, cancellationToken);
    }
}
