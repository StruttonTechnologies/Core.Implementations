using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StruttonTechnologies.Core.Orchestration;

internal static class GenericDtoMapper
{
    private static readonly ConcurrentDictionary<(Type EntityType, Type DtoType), Delegate> _toDtoMappers = new();
    private static readonly ConcurrentDictionary<(Type DtoType, Type EntityType), Delegate> _toEntityMappers = new();

    public static void Register<TEntity, TDto, TKey>(
        Func<TEntity, TDto> toDto,
        Func<TDto, TEntity> toEntity)
        where TKey : IEquatable<TKey>
    {
        _toDtoMappers[(typeof(TEntity), typeof(TDto))] = toDto;
        _toEntityMappers[(typeof(TDto), typeof(TEntity))] = toEntity;
    }

    public static TDto ToDto<TEntity, TDto, TKey>(TEntity entity)
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (_toDtoMappers.TryGetValue((typeof(TEntity), typeof(TDto)), out var mapper))
        {
            return ((Func<TEntity, TDto>)mapper)(entity);
        }

        throw new InvalidOperationException($"No ToDto mapper registered for {typeof(TEntity).Name} ? {typeof(TDto).Name}");
    }

    public static TEntity ToEntity<TEntity, TDto, TKey>(TDto dto)
        where TKey : IEquatable<TKey>
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (_toEntityMappers.TryGetValue((typeof(TDto), typeof(TEntity)), out var mapper))
        {
            return ((Func<TDto, TEntity>)mapper)(dto);
        }

        throw new InvalidOperationException($"No ToEntity mapper registered for {typeof(TDto).Name} ? {typeof(TEntity).Name}");
    }
}
