using FluentAssertions;

using Moq;

using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;
using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;

using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Handlers.CreatePersonCommandHandler.ValidationFailure;

public class ValidationFailureTests
{
    [Fact]
    public async Task Should_ReturnError_When_ValidationFails()
    {
        PersonValidatorClass validator = new PersonValidatorClass();
        Mock<IPersonOrchestration<int>> mockOrchestration = new Mock<IPersonOrchestration<int>>();
        CreatePersonCommandHandler<int> handler = new CreatePersonCommandHandler<int>(validator, mockOrchestration.Object);

        PersonDto dto = new PersonDto
        {
            Id = "0",
            FirstName = null!,
            LastName = "Doe",
            Email = "john.doe@test.com"
        };
        CreatePersonCommand command = new CreatePersonCommand(dto);

        var result = await handler.Handle(command, default);

        result.Success.Should().BeFalse();
        result.Message.Should().Contain("First name is required");
    }

    [Fact]
    public async Task Should_NotCallOrchestration_When_ValidationFails()
    {
        PersonValidatorClass validator = new PersonValidatorClass();
        Mock<IPersonOrchestration<int>> mockOrchestration = new Mock<IPersonOrchestration<int>>();
        CreatePersonCommandHandler<int> handler = new CreatePersonCommandHandler<int>(validator, mockOrchestration.Object);

        PersonDto dto = new PersonDto
        {
            Id = "0",
            FirstName = null!,
            LastName = "Doe",
            Email = "john.doe@test.com"
        };
        CreatePersonCommand command = new CreatePersonCommand(dto);

        await handler.Handle(command, default);

        mockOrchestration.Verify(o => o.CreatePersonAsync(It.IsAny<Person<int>>(), It.IsAny<System.Threading.CancellationToken>()), Times.Never);
    }
}
