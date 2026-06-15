using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.CreatePerson;

public class CancellationTokenTests
{
    [Fact]
    public async Task Should_PassCancellationToken_When_CreatingPerson()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        var person = new Person<int>
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test.user@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };
        var cancellationToken = new CancellationToken();

        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        await service.CreatePersonAsync(person, cancellationToken);

        mockRepository.Verify(r => r.AddAsync(person, cancellationToken), Times.Once);
    }
}
