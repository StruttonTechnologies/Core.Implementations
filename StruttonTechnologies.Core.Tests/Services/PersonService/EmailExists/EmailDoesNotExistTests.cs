using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.EmailExists;

public class EmailDoesNotExistTests
{
    [Fact]
    public async Task Should_ReturnFalse_When_EmailDoesNotExist()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        mockRepository
            .Setup(r => r.EmailExistsAsync("nonexistent@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await service.EmailExistsAsync("nonexistent@test.com");

        result.Should().BeFalse();
    }
}
