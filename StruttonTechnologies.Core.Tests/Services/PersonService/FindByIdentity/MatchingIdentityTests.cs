using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.FindByIdentity;

public class MatchingIdentityTests
{
    [Fact]
    public async Task Should_ReturnPerson_When_IdentityMatches()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        var expectedPerson = new Person<int>
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };

        mockRepository
            .Setup(r => r.FindByIdentityAsync("John", "Doe", "john.doe@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);

        var result = await service.FindByIdentityAsync("John", "Doe", "john.doe@test.com");

        result.Should().NotBeNull();
        result.Should().Be(expectedPerson);
    }

    [Fact]
    public async Task Should_CallRepository_When_FindingByIdentity()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        await service.FindByIdentityAsync("Jane", "Smith", "jane.smith@test.com");

        mockRepository.Verify(r => r.FindByIdentityAsync("Jane", "Smith", "jane.smith@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}
