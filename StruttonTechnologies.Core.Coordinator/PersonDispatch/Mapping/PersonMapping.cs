using StruttonTechnologies.Core.Domain.Entities;
using StruttonTechnologies.Core.Dtos.Person;

namespace StruttonTechnologies.Core.Coordinator.PersonDispatch.Mapping;

public static class PersonExtensions
{
    /// <summary>
    /// Maps a PersonDto to a Person<TKey> entity.
    /// </summary>
    public static Person<TKey> ToEntity<TKey>(this PersonDto dto)
        where TKey : IEquatable<TKey>
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        return new Person<TKey>
        {
            // You may need to convert dto.Id (string/int/etc.) to TKey here
            Id = (TKey)Convert.ChangeType(dto.Id, typeof(TKey)),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            // Map other fields as needed
        };
    }

    /// <summary>
    /// Maps a Person<TKey> entity to a PersonDto.
    /// </summary>
    public static PersonDto ToDto<TKey>(this Person<TKey> entity)
        where TKey : IEquatable<TKey>
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return new PersonDto
        {
            Id = entity.Id?.ToString() ?? string.Empty, // store as string in DTO
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email ?? string.Empty,
            // Map other fields as needed
        };
    }

    public static TEntity ToEntity<TKey, TEntity>(this PersonDto dto)
        where TKey : IEquatable<TKey>
        where TEntity : Person<TKey>, new()
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        TEntity entity = new TEntity();
        entity.Id = (TKey)Convert.ChangeType(dto.Id, typeof(TKey));
        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.Email = dto.Email;
        return entity;
    }
}
