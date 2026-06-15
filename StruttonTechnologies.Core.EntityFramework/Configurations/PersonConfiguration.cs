using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using StruttonTechnologies.Core.Domain.Entities;

namespace StruttonTechnologies.Core.EntityFramework.Configurations;

public class PersonConfiguration<TKey, TPerson> : IEntityTypeConfiguration<TPerson>
    where TKey : IEquatable<TKey>
    where TPerson : Person<TKey>
{
    public void Configure(EntityTypeBuilder<TPerson> builder)
    {
        // Primary key
        builder.HasKey(p => p.Id);

        // Composite unique index (FirstName + LastName + Email)
        builder.HasIndex(p => new { p.FirstName, p.LastName, p.Email })
               .IsUnique();

        // Field constraints
        builder.Property(p => p.FirstName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(p => p.LastName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(p => p.Email)
               .HasMaxLength(256) // match your DataAnnotation
               .IsRequired();

        builder.Property(p => p.PhoneNumber)
               .HasMaxLength(20);

        // Concurrency token
        builder.Property(p => p.RowVersion)
               .IsConcurrencyToken();
    }
}
