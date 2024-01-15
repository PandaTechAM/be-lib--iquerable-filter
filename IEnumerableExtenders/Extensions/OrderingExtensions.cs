using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Enums;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class OrderingExtensions
{
    public static IOrderedQueryable<TModel> ApplyOrdering<TModel, TKey>(this IQueryable<TModel> dbset,
        Ordering ordering, Expression<Func<TModel, TKey>> defaultKeySelector, bool descending = false)
    {
        if (ordering.PropertyName == string.Empty)
            return descending
                ? dbset.OrderByDescending(defaultKeySelector)
                : dbset.OrderBy(defaultKeySelector);


        var targetProperty = typeof(TModel)
                                 .GetTargetType()
                                 .GetProperties()
                                 .FirstOrDefault(x => x.Name == ordering.PropertyName)?
                                 .GetCustomAttribute<MappedToPropertyAttribute>() ??
                             throw new PropertyNotFoundException(ordering.PropertyName);

        var keySelector = PropertyHelper.GetPropertyLambda(targetProperty);

        if (ordering.Descending)
            keySelector += " DESC";

        return dbset.OrderBy(keySelector);
    }

    private class OrderMapping
    {
        public MappedToPropertyAttribute MappedToPropertyAttribute { get; set; }
        public OrderAttribute OrderAttribute { get; set; }
    }

    public static IOrderedQueryable<TModel> ApplyOrdering<TModel>(this IQueryable<TModel> dbset, Ordering ordering)
    {
        var targetType = typeof(TModel).GetTargetType();

        if (ordering.PropertyName != string.Empty)
        {
            var targetProperty = targetType
                                     .GetProperties()
                                     .FirstOrDefault(x => x.Name == ordering.PropertyName)?
                                     .GetCustomAttribute<MappedToPropertyAttribute>() ??
                                 throw new PropertyNotFoundException(ordering.PropertyName);

            var keySelector = PropertyHelper.GetPropertyLambda(targetProperty);

            if (ordering.Descending)
                keySelector += " DESC";

            return dbset.OrderBy(keySelector);
        }

        var properties = targetType.GetProperties()
            .Select(x => new
            {
                MappedToPropertyAttribute = x.GetCustomAttribute<MappedToPropertyAttribute>(),
                OrderAttribute = x.GetCustomAttribute<OrderAttribute>()
            }).Where(x => x.MappedToPropertyAttribute is not null && x.OrderAttribute is not null)
            .Select(x => new OrderMapping
            {
                MappedToPropertyAttribute = x.MappedToPropertyAttribute!,
                OrderAttribute = x.OrderAttribute!
            }).OrderBy(x => x.OrderAttribute.Order).ToList();


        // validate

        if (properties.Count == 0)
            throw new NoOrderingFoundException();

        for (var index = 0; index < properties.Count; index++)
        {
            var property = properties[index];
            if (index+1 != property.OrderAttribute.Order)
                throw new DefaultOrderingViolation();
        }

        var firstProperty = properties[0];

        var q = dbset.OrderBy(PropertyHelper.GetPropertyLambda(firstProperty.MappedToPropertyAttribute)
                              + (firstProperty.OrderAttribute.Direction == OrderDirection.Descending
                                  ? " DESC"
                                  : string.Empty));

        for (var index = 1; index < properties.Count; index++)
        {
            var property = properties[index];
            q = q.ThenBy(PropertyHelper.GetPropertyLambda(property.MappedToPropertyAttribute)
                         + (property.OrderAttribute.Direction == OrderDirection.Descending
                             ? " DESC"
                             : string.Empty));
        }
        
        
        return q;
    }
}

public class DefaultOrderingViolation : FilterException;