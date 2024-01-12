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
            if (targetType.IsIEnumerable())
                targetType = targetType.GetCollectionType();
            var method = typeof(PropertyHelper).GetMethod("GetValues")!.MakeGenericMethod(targetType);
            var values = method.Invoke(null, [filter, mappedToPropertyAttribute]);

            var firstValue = values.GetType().GetMethod("First")!.Invoke(values, null);
            var secondValue = values.GetType().GetMethod("Last")!.Invoke(values, null);
            
            switch (filter.ComparisonType)
            {
                case ComparisonType.Equal:
                case ComparisonType.NotEqual:
                case ComparisonType.GreaterThan:
                case ComparisonType.GreaterThanOrEqual:
                case ComparisonType.LessThan:
                case ComparisonType.LessThanOrEqual:
                case ComparisonType.Contains:
                case ComparisonType.StartsWith:
                case ComparisonType.NotContains:
                case ComparisonType.EndsWith:
                    q = q.Where(lambda, firstValue);
                    break;
                case ComparisonType.In:
                    q = q.Where(lambda, values);
                    break;
                case ComparisonType.NotIn:
                    q = q.Where(lambda, values);
                    break;
                case ComparisonType.IsNotEmpty:
                case ComparisonType.IsEmpty:
                    q = q.Where(lambda);
                    break;
                case ComparisonType.Between:
                    q = q.Where(lambda,firstValue,secondValue);
                    break;
                case ComparisonType.HasCountEqualTo:
                    q = q.Where(lambda, firstValue);
                    break;
                case ComparisonType.HasCountBetween:
                    q = q.Where(lambda,firstValue,secondValue);
                    break;
                case ComparisonType.IsTrue:
                    q = q.Where(lambda);
                    break;
                case ComparisonType.IsFalse:
                    q = q.Where(lambda);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        return q;
    }
}