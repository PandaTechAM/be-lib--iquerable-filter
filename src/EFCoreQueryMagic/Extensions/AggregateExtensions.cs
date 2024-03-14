using System.Linq.Expressions;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Extensions;

public static class AggregateExtensions
{
    public static async Task<object?> AggregateAsync<TModel>(
        this IQueryable<TModel> dbSet,
        AggregateDto aggregate, CancellationToken cancellationToken = default) where TModel : class
    {
        var dto = Attributes.FilterModelAttributeHelper.GetTargetType(typeof(TModel)) ;
        
        var sourceProperty = dto.GetProperty(aggregate.PropertyName);
        if (sourceProperty is null)
        {
            throw new PropertyNotFoundException(
                $"Property {aggregate.PropertyName} not found in {dto.Name}");
        }

        var attribute = sourceProperty.GetCustomAttribute<MappedToPropertyAttribute>();
        if (attribute is null)
        {
            throw new PropertyNotFoundException(
                $"Property {aggregate.PropertyName} not mapped in {dto.Name}");
        }

        // TODO: Add aggregate availability check in MappedToPropertyAttribute

        var targetProperty = typeof(TModel).GetProperty(attribute.TargetPropertyName);
        if (targetProperty is null)
        {
            throw new PropertyNotFoundException(
                $"Property {attribute.TargetPropertyName} not found in {typeof(TModel).Name}");
        }

        var parameter = Expression.Parameter(typeof(TModel));
        
        var propertyType = PropertyHelper.GetPropertyType(typeof(TModel), attribute);
        var propertyAccess = PropertyHelper.GetPropertyExpression(parameter, attribute);
        
        if (propertyType == typeof(string))
        {
            var lambda = Expression.Lambda<Func<TModel, string>>(propertyAccess, parameter);

            if (aggregate.AggregateType == AggregateType.UniqueCount)
            {
                return await dbSet.Select(lambda).Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken);
            }
            return null;
        }

        if (propertyType == typeof(int))
        {
            var lambda = Expression.Lambda<Func<TModel, int>>(propertyAccess, parameter);

            return aggregate.AggregateType switch
            {
                AggregateType.UniqueCount => await dbSet.Select(lambda)
                    .Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken),
                AggregateType.Sum => await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                AggregateType.Average => await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                AggregateType.Min => await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                AggregateType.Max => await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                _ => throw new ArgumentOutOfRangeException(paramName: "", message: "Unknown aggregate type")
            };
        }

        if (propertyType == typeof(long))
        {
            var lambda = Expression.Lambda<Func<TModel, long>>(propertyAccess, parameter);

            return aggregate.AggregateType switch
            {
                AggregateType.UniqueCount => await dbSet.Select(lambda)
                    .Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken),
                AggregateType.Sum => await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                AggregateType.Average => await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                AggregateType.Min => await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                AggregateType.Max => await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                _ => throw new ArgumentOutOfRangeException(paramName: "", message: "Unknown aggregate type")
            };
        }

        if (propertyType == typeof(DateTime))
        {
            var lambda = Expression.Lambda<Func<TModel, DateTime>>(propertyAccess, parameter);
            return aggregate.AggregateType switch
            {
                AggregateType.Min => await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                AggregateType.Max => await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                _ => null
            };
        }

        if (propertyType == typeof(decimal))
        {
            var lambda = Expression.Lambda<Func<TModel, decimal>>(propertyAccess, parameter);

            return aggregate.AggregateType switch
            {
                AggregateType.UniqueCount => await dbSet.Select(lambda)
                    .Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken),
                AggregateType.Sum => await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                AggregateType.Average => await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                AggregateType.Min => await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                AggregateType.Max => await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                _ => throw new ArgumentOutOfRangeException(paramName: "", message: "Unknown aggregate type")
            };
        }

        if (propertyType == typeof(double))
        {
            var lambda = Expression.Lambda<Func<TModel, double>>(propertyAccess, parameter);

            return aggregate.AggregateType switch
            {
                AggregateType.UniqueCount => await dbSet.Select(lambda)
                    .Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken),
                AggregateType.Sum => await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                AggregateType.Average => await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                AggregateType.Min => await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                AggregateType.Max => await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                _ => throw new ArgumentOutOfRangeException(paramName: "", message: "Unknown aggregate type")
            };
        }

        if (propertyType == typeof(Guid))
        {
            var lambda = Expression.Lambda<Func<TModel, Guid>>(propertyAccess, parameter);

            return aggregate.AggregateType switch
            {
                AggregateType.UniqueCount => await dbSet.Select(lambda)
                    .Distinct()
                    .LongCountAsync(cancellationToken: cancellationToken),
                _ => null
            };
        }

        if (propertyType.IsClass)
        {
            //TODO: Sub property 

            throw new NotImplementedException();
        }

        if (propertyType == typeof(byte[]))
        {
            // TODO: check if is encrypted field.
            // if so - build proper lamda
            throw new NotImplementedException();
        }

        return null;
    }
    
    public static Task<object?> AggregateAsync<TModel>(this IQueryable<TModel> dbSet,
        string columnName, AggregateType aggregateType, CancellationToken cancellationToken = default) where TModel : class
    {
        return dbSet.AggregateAsync(new AggregateDto { AggregateType = aggregateType, PropertyName = columnName },
            cancellationToken: cancellationToken);
    }
}