using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Repositories.Crud;
using StruttonTechnologies.Core.Repositories.Person;
using StruttonTechnologies.Core.Tests.TestHelpers;

namespace StruttonTechnologies.Core.Tests.Repositories.PersonRepository.GetByEmail;

public class ExistingEmailTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly PersonRepository<Person<int>, int> _repository;

    public ExistingEmailTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        var mockLogger = new Mock<ILogger<CrudRepository<Person<int>, int>>>();
        _repository = new PersonRepository<Person<int>, int>(_context, mockLogger.Object);
    }

    [Fact]
    public async Task Should_ReturnPerson_When_EmailExists()
    {
        var person = CreateTestPerson("test@test.com");
        _context.Set<Person<int>>().Add(person);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByEmailAsync("test@test.com");

        result.Should().NotBeNull();
        result!.Email.Should().Be("test@test.com");
    }

    private static Person<int> CreateTestPerson(string email)
    {
        return new Person<int>
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            CreatedAt = DateTime.UtcNow,
            RowVersion = new byte[] { 1 }
        };
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
