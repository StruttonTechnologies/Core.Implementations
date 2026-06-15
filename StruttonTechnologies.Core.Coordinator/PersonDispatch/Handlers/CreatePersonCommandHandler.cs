using MediatR;
using StruttonTechnologies.Core.Coordinator.Contracts.Models;
using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Mapping;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;

public class CreatePersonCommandHandler<TKey>
: IRequestHandler<CreatePersonCommand, TaskResult<PersonDto>>
where TKey : IEquatable<TKey>
{
    private readonly PersonValidator _validator;
    private readonly IPersonOrchestration<TKey> _orchestration;

    public CreatePersonCommandHandler(PersonValidator validator, IPersonOrchestration<TKey> orchestration)
    {
        _validator = validator;
        _orchestration = orchestration;
    }

    public async Task<TaskResult<PersonDto>> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        // Run validation
        var validation = _validator.Validate(request.Payload);
        if (!validation.IsValid)
            return TaskResult<PersonDto>.Error(
                validation.Message ?? "Validation failed",
                "ValidationError"
            );

        // Map DTO ? Entity
        var person = request.Payload.ToEntity<TKey>();

        // Orchestrate creation
        var personEntity = await _orchestration.CreatePersonAsync(person, cancellationToken);
        if (personEntity is null)
            return TaskResult<PersonDto>.NotFound("Person could not be created");

        // Map Entity ? DTO
        return TaskResult<PersonDto>.Ok(personEntity.ToDto(), "Person created successfully");
    }
}
