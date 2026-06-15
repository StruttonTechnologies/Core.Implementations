using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.GetByEmail;

public class NonExistentEmailTests
{
    [Fact]
    public async Task Should_ReturnNull_When_EmailDoesNotExist()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        mockRepository
            .Setup(r => r.GetByEmailAsync("nonexistent@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person<int>?)null);

        var result = await service.GetByEmailAsync("nonexistent@test.com");

        result.Should().BeNull();
    }
}
