using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.EmailExists;

public class EmailExistsTests
{
    [Fact]
    public async Task Should_ReturnTrue_When_EmailExists()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        mockRepository
            .Setup(r => r.EmailExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await service.EmailExistsAsync("existing@test.com");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task Should_CallRepository_When_CheckingEmailExists()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        await service.EmailExistsAsync("test@test.com");

        mockRepository.Verify(r => r.EmailExistsAsync("test@test.com", It.IsAny<CancellationToken>()), Times.Once);
    }
}
