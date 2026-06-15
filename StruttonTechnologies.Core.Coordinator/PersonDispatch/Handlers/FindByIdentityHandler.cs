using MediatR;
using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Mapping;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;
using System;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;

/// <summary>
/// Handles FindByIdentityQuery requests by delegating to the orchestration layer.
/// </summary>
public class FindByIdentityHandler<TKey>
    : IRequestHandler<FindByIdentityQuery, PersonDto?>
    where TKey : IEquatable<TKey>
{
    private readonly IPersonOrchestration<TKey> _service;

    public FindByIdentityHandler(IPersonOrchestration<TKey> service)
    {
        _service = service;
    }

    public async Task<PersonDto?> Handle(FindByIdentityQuery request, CancellationToken cancellationToken)
    {
        var person = await _service.FindByIdentityAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            cancellationToken);

        return person?.ToDto();
    }
    
}
