using Microsoft.EntityFrameworkCore;

using StruttonTechnologies.Core.Domain.Entities;

namespace StruttonTechnologies.Core.Tests.TestHelpers;

public class TestDbContext : DbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
    {
    }

    public DbSet<Person<int>> Persons => Set<Person<int>>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Person<int>>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.RowVersion).IsRowVersion();
        });
    }
}
