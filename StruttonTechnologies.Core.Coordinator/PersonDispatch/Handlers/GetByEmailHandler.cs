using MediatR;
using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Mapping; // for ToDto extension
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;
using System;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;

/// <summary>
/// Handles GetByEmailQuery requests by delegating to the orchestration layer.
/// </summary>
public class GetByEmailHandler<TKey>
    : IRequestHandler<GetByEmailQuery, PersonDto?>
    where TKey : IEquatable<TKey>
{
    private readonly IPersonOrchestration<TKey> _service;

    public GetByEmailHandler(IPersonOrchestration<TKey> service)
    {
        _service = service;
    }

    public async Task<PersonDto?> Handle(GetByEmailQuery request, CancellationToken cancellationToken)
    {
        var person = await _service.GetByEmailAsync(request.Email, cancellationToken);
        return person?.ToDto(); // centralized mapping extension
    }
}
