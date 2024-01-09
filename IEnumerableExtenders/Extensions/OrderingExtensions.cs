using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class OrderingExtensions
{
    public static IQueryable<TModel> ApplyOrdering<TModel, TDto>(this IQueryable<TModel> dbSet, Ordering ordering)
    {
        if (ordering.PropertyName == string.Empty)
            return dbSet;

        var mappedProperties = typeof(TDto).GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .ToDictionary(
                x => x.Name,
                x => new
                {
                    x.GetCustomAttribute<MappedToPropertyAttribute>()!.TargetPropertyName,
                    x.GetCustomAttribute<MappedToPropertyAttribute>()!.Sortable
                }
            );

        var filter = mappedProperties[ordering.PropertyName];

        if (!filter.Sortable)
            throw new OrderingDeniedException("Property " + ordering.PropertyName + " is not sortable");

        return ordering is { Descending: false }
            ? dbSet.OrderBy(filter.TargetPropertyName)
            : dbSet.OrderBy(filter.TargetPropertyName + " DESC");
    }

    public static IOrderedQueryable<TModel> ApplyOrdering<TModel, TDto>(this IOrderedQueryable<TModel> dbSet,
        Ordering ordering)
    {
        if (ordering.PropertyName == string.Empty)
            return dbSet;

        var mappedProperties = typeof(TDto).GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .ToDictionary(
                x => x.Name,
                x => new
                {
                    x.GetCustomAttribute<MappedToPropertyAttribute>()!.TargetPropertyName,
                    x.GetCustomAttribute<MappedToPropertyAttribute>()!.Sortable
                }
            );

        var filter = mappedProperties[ordering.PropertyName];

        if (!filter.Sortable)
            throw new OrderingDeniedException("Property " + ordering.PropertyName + " is not sortable");

        return ordering is { Descending: false }
            ? dbSet.OrderBy(filter.TargetPropertyName)
            : dbSet.OrderBy(filter.TargetPropertyName + " DESC");
    }

    public static IQueryable<TModel> ApplyOrdering<TModel, TDto>(this IEnumerable<TModel> dbSet, Ordering ordering)
    {
        return dbSet.AsQueryable().ApplyOrdering<TModel, TDto>(ordering);
    }

    public static IQueryable<TModel> ApplyOrdering<TModel, TDto>(this DbSet<TModel> dbSet, Ordering ordering)
        where TModel : class
    {
        return dbSet.AsQueryable().ApplyOrdering<TModel, TDto>(ordering);
    }
}