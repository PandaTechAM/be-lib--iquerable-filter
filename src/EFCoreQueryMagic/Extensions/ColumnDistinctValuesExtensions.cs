using System.Linq.Dynamic.Core;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Extensions;

internal static class ColumnDistinctValuesExtensions
{
    private static IQueryable<object> GenerateBaseQueryable<TModel>(this IQueryable<TModel> dbSet,
        List<FilterQuery> filters, DbContext? context = null) where TModel : class
    {
        var query = dbSet
            .ApplyFilters(filters, context);

        return query;
    }

    private static IQueryable<object> GenerateBaseQueryable<TModel>(this IQueryable<TModel> dbSet,
        MagicQuery request, DbContext? context = null) where TModel : class
    {
        var query = dbSet
            .ApplyFilters(request.Filters, context)
            .ApplyOrdering(request.Order);

        return query;
    }

    internal static async Task<ColumnDistinctValues> DistinctColumnValuesAsync<TModel>(
        this IQueryable<TModel> dbSet, List<FilterQuery>? filters, MagicQuery? request,
        string columnName, int pageSize, int page, DbContext? context = null,
        CancellationToken cancellationToken = default) where TModel : class
    {
        var result = new ColumnDistinctValues();

        var targetProperty = typeof(TModel)
            .GetTargetType()
            .GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .FirstOrDefault(x => x.Name == columnName);

        if (targetProperty is null)
            throw new PropertyNotFoundException($"Property {columnName} not found in {typeof(TModel).Name}");

        var mappedToPropertyAttribute = targetProperty.GetCustomAttribute<MappedToPropertyAttribute>()!;

        var propertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);

        var query = Enumerable.Empty<object>().AsQueryable();

        if (filters is not null && request is null)
        {
            query = GenerateBaseQueryable(dbSet, filters, context);
        }

        if (filters is null && request is not null)
        {
            query = GenerateBaseQueryable(dbSet, request, context);
        }

        IQueryable<object> query2;

        var property = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute);

        if (propertyType.IsIEnumerable() && !mappedToPropertyAttribute.Encrypted)
        {
            if (propertyType.GetCollectionType().IsPrimitive)
            {
                throw new UnsupportedFilterException("Primitive collections are not supported for distinct values."); 
            }

            if (propertyType.GetCollectionType().IsEnum)
            {
                var vals = Enum.GetValues(propertyType.GetCollectionType());
                
                foreach (var val in vals)
                {
                    if (dbSet.Any($"x => x.{property}.Contains(@0)", val))
                    {
                        result.Values.Add(val);
                    }
                }
                
                result.TotalCount = result.Values.Count;
                return result;
            }

            query2 = query.SelectMany<object>(property);
        }
        else
        {
            query2 = query.Select<object>(property);
        }

        var converter = (mappedToPropertyAttribute.Encrypted
                ? Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(EncryptedConverter))
                : Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(DirectConverter))) as
            IConverter;

        converter!.Context = context;

        var method = converter.GetType().GetMethods().First(x => x.Name == "ConvertFrom");

        var query3 = query2.Distinct();
        query3 = query3.OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x);

        List<object> queried;

        if (propertyType.EnumCheck())
        {
            var excludedValues = Enum.GetValues(propertyType.GetCollectionType()).Cast<object>()
                .Where(x => (x as Enum)!.HasAttributeOfType<HideEnumValueAttribute>())
                .ToList();

            queried = query3.ToList()
                .Where(x => !excludedValues.Contains(x))
                // .Skip(pageSize * (page - 1))
                // .Take(pageSize)
                .ToList();
        }
        else
        {
            var paged = query3.Skip(pageSize * (page - 1)).Take(pageSize);
            if (paged is IAsyncEnumerable<object>)
            {
                queried = await paged.ToListAsync(cancellationToken: cancellationToken);
            }
            else
            {
                queried = paged.ToList();
            }
        }

        var converted = queried.Select(x => method.Invoke(converter, [x])!);

        var values = converted.Distinct();
        if (converter is not FilterPandaBaseConverter && method.ReturnType.IsSubclassOf(typeof(IComparable)))
        {
            values = values.OrderBy(x => x == null ? 0 : 1).ThenBy(x => x);
        }

        result.Values = values.ToList();

        try
        {
            result.TotalCount = mappedToPropertyAttribute.Encrypted
                ? 1
                : query3 is IAsyncEnumerable<object>
                    ? await query3.LongCountAsync(cancellationToken: cancellationToken)
                    : query3.LongCount();
        }
        catch
        {
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : 0;
        }

        return result;
    }
}