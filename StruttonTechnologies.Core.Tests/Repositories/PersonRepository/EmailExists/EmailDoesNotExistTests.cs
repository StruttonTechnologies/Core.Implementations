using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Repositories.Person;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.PersonRepository.EmailExists;

public class EmailDoesNotExistTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PersonRepository<Person<int>, int> _repository;

    public EmailDoesNotExistTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new PersonRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnFalse_When_EmailDoesNotExist()
    {
        var result = await _repository.EmailExistsAsync("nonexistent@test.com", default);

        result.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
