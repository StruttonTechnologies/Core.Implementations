using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.GetByEmail;

public class ExistingEmailTests
{
    [Fact]
    public async Task Should_ReturnPerson_When_EmailExists()
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
            .Setup(r => r.GetByEmailAsync("john.doe@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedPerson);

        var result = await service.GetByEmailAsync("john.doe@test.com");

        result.Should().NotBeNull();
        result.Should().Be(expectedPerson);
    }

    [Fact]
    public async Task Should_CallRepository_When_GettingByEmail()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        await service.GetByEmailAsync("test@test.com");

        mockRepository.Verify(r => r.GetByEmailAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}
