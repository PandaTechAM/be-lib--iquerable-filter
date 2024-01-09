using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class OrderingExtensions
{
   public static IOrderedQueryable<TModel> ApplyOrdering<TModel, TKey>(this IQueryable<TModel> dbset,
        Ordering ordering,
        Expression<Func<TModel, TKey>> defaultKeySelector, bool descending = false)
    {
        if (ordering.PropertyName == string.Empty)
            return descending
                ? dbset.OrderByDescending(defaultKeySelector)
                : dbset.OrderBy(defaultKeySelector);


        var targetProperty = typeof(TModel)
            .GetCustomAttribute<FilterModelAttribute>()?.TargetType
            .GetProperties()
            .FirstOrDefault(x => x.Name == ordering.PropertyName)?
            .GetCustomAttribute<MappedToPropertyAttribute>() ?? throw new PropertyNotFoundException(ordering.PropertyName);

        var keySelector = PropertyHelper.GetPropertyLambda(targetProperty);
        
        if (descending)
            keySelector += " DESC";
        
        return dbset.OrderBy(keySelector);

    }
    
}