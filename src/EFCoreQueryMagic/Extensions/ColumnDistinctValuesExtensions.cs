using System.Collections;
using System.Linq.Dynamic.Core;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Extensions;

internal static class ColumnDistinctValuesExtensions
{
    private static IQueryable<object> GenerateBaseQueryable<TModel>(this IQueryable<TModel> dbSet,
        List<FilterQuery> filters) where TModel : class
    {
        var query = dbSet
            .ApplyFilters(filters);

        return query;
    }

    private static IQueryable<object> GenerateBaseQueryable<TModel>(this IQueryable<TModel> dbSet,
        MagicQuery request) where TModel : class
    {
        var query = dbSet
            .ApplyFilters(request.Filters)
            .ApplyOrdering(request.Order);

        return query;
    }

    static Type GetEnumerableType(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return type.GetGenericArguments()[0];

        if (type.IsArray)
            return type.GetElementType()!;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];

        if (type.IsEnum)
            return type;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return type.GetGenericArguments()[0];

        return type;
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
            query = GenerateBaseQueryable(dbSet, filters);
        }

        if (filters is null && request is not null)
        {
            query = GenerateBaseQueryable(dbSet, request);
        }

        IQueryable<object> query2;

        var property = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute);

        if (propertyType.IsIEnumerable() && !mappedToPropertyAttribute.Encrypted)
        {
            var collections = ((IQueryable<object>)query.Select(property))
                .ToList()
                .Select(x => x as IEnumerable);

            var finalResult = new HashSet<object>();
            foreach (var collection in collections)
            {
                if (collection is null)
                {
                    finalResult.Add(null);
                    continue;
                }

                foreach (var nest in collection)
                {
                    finalResult.Add(nest);
                }
            }

            query2 = finalResult.AsQueryable();
            //query2 = (IQueryable<object>)query.Select(property).SelectMany("x => x");
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

        var query3 = query2.Distinct()
            .OrderBy(x => 1)
            .ThenBy(x => x);

        List<object> queried;

        if (propertyType.EnumCheck())
        {
            var excludedValues = Enum.GetValues(GetEnumerableType(propertyType)).Cast<object>()
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
        if (converter is not FilterPandaBaseConverter)
        {
            values = values.OrderBy(x => x);
        }

        result.Values = values.ToList();

        try
        {
            result.TotalCount = mappedToPropertyAttribute.Encrypted
                ? 1
                : await query3.LongCountAsync(cancellationToken: cancellationToken);
        }
        catch
        {
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : 0;
        }

        return result;
    }
}