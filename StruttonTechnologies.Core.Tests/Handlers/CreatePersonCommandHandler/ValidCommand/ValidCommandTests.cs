using FluentAssertions;

using Moq;

using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;
using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;

using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Handlers.CreatePersonCommandHandler.ValidCommand;

public class ValidCommandTests
{
    [Fact]
    public async Task Should_ReturnSuccess_When_CommandIsValid()
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

        Person<int> createdPerson = new Person<int>
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };

        mockOrchestration
            .Setup(o => o.CreatePersonAsync(It.IsAny<Person<int>>(), It.IsAny<System.Threading.CancellationToken>()))
            .ReturnsAsync(createdPerson);

        var result = await handler.Handle(command, default);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.FirstName.Should().Be("John");
        result.Data.LastName.Should().Be("Doe");
    }
}
