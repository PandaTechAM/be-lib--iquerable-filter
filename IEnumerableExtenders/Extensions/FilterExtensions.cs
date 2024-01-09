using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class FilterExtensions
{
    //public static IQueryable<TModel> ApplyFilters<TModel, TDto>(this IQueryable<TModel> dbSet, List<FilterDto> filters)
    public static IQueryable<TModel> ApplyFilters<TModel>(this IQueryable<TModel> dbSet, List<FilterDto> filters)
    {
        var q = dbSet;

        var filterClassAttribute = typeof(TModel).GetTargetType();

        foreach (var filter in filters)
        {
            var filterProperty = filterClassAttribute.GetProperty(filter.PropertyName);
            if (filterProperty is null)
                throw new PropertyNotFoundException(
                    $"Property {filter.PropertyName} not mapped in {typeof(TModel).Name}");

            var mappedToPropertyAttribute = filterProperty?.GetCustomAttribute<MappedToPropertyAttribute>();
            if (mappedToPropertyAttribute is null)
                throw new PropertyNotFoundException(
                    $"Property {filter.PropertyName} not mapped in {typeof(TModel).Name}");


            var lambda = FilterLambdaBuilder.BuildLambdaString(new FilterKey
            {
                ComparisonType = filter.ComparisonType,
                TargetPropertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                TargetPropertyName = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute),
                SourcePropertyName = filter.PropertyName,
                TargetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                SourceType = filterProperty!.PropertyType,
                SourcePropertyType = filterProperty.PropertyType
            });

            var targetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);
            var method = typeof(PropertyHelper).GetMethod("GetValues")!.MakeGenericMethod(targetType);
            var values = method.Invoke(null, [filter, mappedToPropertyAttribute]);

            q = q.Where(lambda, values);
        }

        return q;
    }
}