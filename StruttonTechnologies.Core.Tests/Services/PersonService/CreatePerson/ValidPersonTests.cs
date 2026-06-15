using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.CreatePerson;

public class ValidPersonTests
{
    [Fact]
    public async Task Should_ReturnCreatedPerson_When_PersonIsValid()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        var person = new Person<int>
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };

        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await service.CreatePersonAsync(person);

        result.Should().NotBeNull();
        result.Should().Be(person);
    }

    [Fact]
    public async Task Should_CallRepositoryAddAsync_When_CreatingPerson()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        var person = new Person<int>
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };

        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        await service.CreatePersonAsync(person);

        mockRepository.Verify(r => r.AddAsync(person, It.IsAny<CancellationToken>()), Times.Once);
    }
}
