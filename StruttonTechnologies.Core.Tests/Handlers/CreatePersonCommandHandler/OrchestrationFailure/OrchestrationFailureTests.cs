using FluentAssertions;

using Moq;

using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;
using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;

using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Handlers.CreatePersonCommandHandler.OrchestrationFailure;

public class OrchestrationFailureTests
{
    [Fact]
    public async Task Should_ReturnNotFound_When_OrchestrationReturnsNull()
    {
        PersonValidatorClass validator = new PersonValidatorClass();
        Mock<IPersonOrchestration<int>> mockOrchestration = new Mock<IPersonOrchestration<int>>();
        CreatePersonCommandHandler<int> handler = new CreatePersonCommandHandler<int>(validator, mockOrchestration.Object);

        PersonDto dto = new PersonDto
        {
            Id = "0",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com"
        };
        CreatePersonCommand command = new CreatePersonCommand(dto);

        mockOrchestration
            .Setup(o => o.CreatePersonAsync(It.IsAny<Person<int>>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync((Person<int>?)null);

        var result = await handler.Handle(command, default);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Person could not be created");
    }
}
