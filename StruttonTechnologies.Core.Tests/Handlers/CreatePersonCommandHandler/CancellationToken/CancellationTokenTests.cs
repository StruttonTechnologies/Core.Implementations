using Moq;

using StruttonTechnologies.Core.Coordinator.Contracts.PersonDispatch;
using StruttonTechnologies.Core.Coordinator.PersonDispatch.Handlers;
using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Dtos.Person;
using StruttonTechnologies.Core.Orchestration.Contracts;

using PersonValidatorClass = StruttonTechnologies.Core.Coordinator.PersonDispatch.Validation.PersonValidator;

namespace StruttonTechnologies.Core.Tests.Handlers.CreatePersonCommandHandler.CancellationToken;

public class CancellationTokenTests
{
    [Fact]
    public async Task Should_PassCancellationToken_When_CallingOrchestration()
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
        System.Threading.CancellationToken cancellationToken = new System.Threading.CancellationToken();

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

        await handler.Handle(command, cancellationToken);

        mockOrchestration.Verify(o => o.CreatePersonAsync(It.IsAny<Person<int>>(), cancellationToken), Times.Once);
    }
}
