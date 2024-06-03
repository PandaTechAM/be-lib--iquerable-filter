using System.Linq.Dynamic.Core;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Helpers;

namespace EFCoreQueryMagic.Extensions;

internal static class OrderingExtensions
{
    private class OrderMapping
    {
        public MappedToPropertyAttribute MappedToPropertyAttribute { get; set; }
        public OrderAttribute OrderAttribute { get; set; }
    }

    internal static IOrderedQueryable<TModel> ApplyOrdering<TModel>(this IQueryable<TModel> query, Ordering? ordering)
    {
        var targetType = typeof(TModel).GetTargetType();

        if (ordering is not null && ordering.PropertyName != string.Empty)
        {
            var targetProperty = targetType
                                     .GetProperties()
                                     .FirstOrDefault(x => x.Name == ordering.PropertyName)?
                                     .GetCustomAttribute<MappedToPropertyAttribute>() ??
                                 throw new PropertyNotFoundException(ordering.PropertyName);

            var keySelector = PropertyHelper.GetPropertyLambda(targetProperty);

            if (ordering.Descending)
                keySelector += " DESC";

            return query.OrderBy(keySelector);
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

        if (properties.Count == 0)
            throw new NoOrderingFoundException();

        for (var index = 0; index < properties.Count; index++)
        {
            var property = properties[index];
            if (index + 1 != property.OrderAttribute.Order)
                throw new DefaultOrderingViolation();
        }

        var firstProperty = properties[0];

        var orderedQuery = query.OrderBy(PropertyHelper.GetPropertyLambda(firstProperty.MappedToPropertyAttribute)
                              + (firstProperty.OrderAttribute.Direction == OrderDirection.Descending
                                  ? " DESC"
                                  : string.Empty));

        for (var index = 1; index < properties.Count; index++)
        {
            var property = properties[index];
            orderedQuery = orderedQuery.ThenBy(PropertyHelper.GetPropertyLambda(property.MappedToPropertyAttribute)
                         + (property.OrderAttribute.Direction == OrderDirection.Descending
                             ? " DESC"
                             : string.Empty));
        }

        return orderedQuery;
    }
}