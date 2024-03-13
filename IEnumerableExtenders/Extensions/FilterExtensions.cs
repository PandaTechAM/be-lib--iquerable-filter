using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;
using PandaTech.IEnumerableFilters.PostgresContext;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class FilterExtensions
{
    //public static IQueryable<TModel> ApplyFilters<TModel, TDto>(this IQueryable<TModel> dbSet, List<FilterDto> filters)
    public static IQueryable<TModel> ApplyFilters<TModel>(this IQueryable<TModel> dbSet, List<FilterDto> filters,
        DbContext? context = null)
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


            var targetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);
            if (targetType.IsIEnumerable() && !mappedToPropertyAttribute.Encrypted)
                targetType = targetType.GetCollectionType();
            var method = typeof(PropertyHelper).GetMethod("GetValues")!.MakeGenericMethod(targetType);
            var values = method.Invoke(null, [filter, mappedToPropertyAttribute, context]);

            if (mappedToPropertyAttribute.Encrypted)
            {
                q = q.Where(EncryptedHelper.GetExpression<TModel>(mappedToPropertyAttribute,
                    (values as List<byte[]>)[0]));
                continue;
            }

            var lambda = FilterLambdaBuilder.BuildLambdaString(new FilterKey
            {
                ComparisonType = filter.ComparisonType,
                TargetPropertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                TargetPropertyName = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute),
                SourcePropertyName = filter.PropertyName,
                TargetType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute),
                SourceType = filterProperty!.PropertyType,
                SourcePropertyType = filterProperty.PropertyType,
            });

            q = q.Where(lambda, values);
        }

        return q;
    }
}

static class EncryptedHelper
{
    public static Expression<Func<TModel, bool>> GetExpression<TModel>(MappedToPropertyAttribute attribute,
        byte[]? value)
    {
        if (value is null )
        {
            var parameter = Expression.Parameter(typeof(TModel));
            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);
            var equality = Expression.Equal(accessor, Expression.Constant(null));
            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }
        else if (value.Length == 0)
        {
            var parameter = Expression.Parameter(typeof(TModel));
            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);
            var equality = Expression.Equal(accessor, Expression.Constant(null));
            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }
        else
        {
            var parameter = Expression.Parameter(typeof(TModel));

            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);

            var postgresFunc =
                typeof(PostgresDbContext).GetMethod("substr", [typeof(byte[]), typeof(int), typeof(int)])!;

            var propertyAccess =
                Expression.Call(postgresFunc, accessor, Expression.Constant(1), Expression.Constant(64));

            var constant = Expression.Constant(value.Take(64).ToArray());

            var equality = Expression.Equal(propertyAccess, constant);

            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }
    }
}