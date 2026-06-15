using FluentAssertions;

using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Orchestration.Services;
using StruttonTechnologies.Core.Repositories.Contracts;

namespace StruttonTechnologies.Core.Tests.Services.PersonService.CreatePerson;

public class ExceptionHandlingTests
{
    [Fact]
    public async Task Should_PropagateException_When_RepositoryThrows()
    {
        var mockRepository = new Mock<IPersonRepository<Person<int>, int>>();
        var mockLogger = new Mock<ILogger<PersonService<int>>>();
        var service = new PersonService<int>(mockRepository.Object, mockLogger.Object);

        var person = new Person<int>
        {
            FirstName = "Error",
            LastName = "Test",
            Email = "error.test@test.com",
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };

        mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Person<int>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database error"));

        var act = async () => await service.CreatePersonAsync(person);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Database error");
    }
}
