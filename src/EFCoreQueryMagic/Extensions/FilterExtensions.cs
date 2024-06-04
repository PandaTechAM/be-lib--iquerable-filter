using System.Linq.Dynamic.Core;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Extensions;

internal static class FilterExtensions
{
    internal static IQueryable<TModel> ApplyFilters<TModel>(this IQueryable<TModel> query, List<FilterQuery> filters, DbContext? context = null)
    {
        var filterClassAttribute = typeof(TModel).GetTargetType();

        foreach (var filter in filters)
        {
            var filterProperty = filterClassAttribute.GetProperty(filter.PropertyName);
            if (filterProperty is null)
            {
                throw new PropertyNotFoundException(
                    $"Property {filter.PropertyName} not mapped in {typeof(TModel).Name}");
            }

            var mappedToPropertyAttribute = filterProperty.GetCustomAttribute<MappedToPropertyAttribute>();

            if (mappedToPropertyAttribute is null)
            {
                throw new PropertyNotFoundException(
                    $"Property {filter.PropertyName} not mapped in {typeof(TModel).Name}");
            }

            var targetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);
            if (targetType.IsIEnumerable() && !mappedToPropertyAttribute.Encrypted)
            {
                targetType = targetType.GetCollectionType();
            }

            var method = typeof(PropertyHelper).GetMethod("GetValues")!.MakeGenericMethod(targetType);

            object? values;
            try
            {
                values = method.Invoke(null, [filter, mappedToPropertyAttribute, context]);
            }
            catch (InvalidOperationException e)
            {
                query = query.Where(x => false);
                continue;
            }
            catch (Exception e)
            {
                throw e.InnerException!;
            }

            if (filter.Values.Count == 0)
            {
                return query.Where(x => false);
            }

            if (mappedToPropertyAttribute.Encrypted)
            {
                query = query.Where(EncryptedHelper.GetExpression<TModel>(mappedToPropertyAttribute,
                    (values as List<byte[]>)![0]));
                continue;
            }

            var lambda = FilterLambdaBuilder.BuildLambdaString(new FilterKey
            {
                ComparisonType = filter.ComparisonType,
                TargetPropertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                TargetPropertyName = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute),
                SourcePropertyName = filter.PropertyName,
                TargetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                SourceType = filterProperty.PropertyType,
                SourcePropertyType = filterProperty.PropertyType,
            });

            query = query.Where(lambda, values);
        }

        return query;
    }
}