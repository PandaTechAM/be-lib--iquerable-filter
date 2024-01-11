using System.Collections;
using System.Linq.Dynamic.Core;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Converters;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;
using PandaTech.IEnumerableFilters.Helpers;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class DistinctColumnValuesExtensions
{
    private static IQueryable<object> GenerateBaseQueryable<TModel>(this IQueryable<TModel> dbSet,
        List<FilterDto> filters) where TModel : class
    {
        var query = dbSet.ApplyFilters(filters);


        return query;
    }

    static List<T> Paginate<T>(this List<T> list, int pageSize, int page)
    {
        return list.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
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

    public static DistinctColumnValuesResult DistinctColumnValues<TModel>(this IQueryable<TModel> dbSet,
        List<FilterDto> filters, string columnName, int pageSize, int page) where TModel : class
    {
        var result = new DistinctColumnValuesResult();

        var targetProperty = typeof(TModel).GetTargetType().GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .FirstOrDefault(x => x.Name == columnName);

        if (targetProperty is null)
            throw new PropertyNotFoundException($"Property {columnName} not found in {typeof(TModel).Name}");

        var mappedToPropertyAttribute = targetProperty.GetCustomAttribute<MappedToPropertyAttribute>()!;
        if (mappedToPropertyAttribute.Encrypted)
            return new();

        var propertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);

        if (propertyType.EnumCheck())
        {
            var values =  Enum.GetValues(GetEnumerableType(propertyType)).Cast<object>().Where(x => !(x as Enum)!.HasAttributeOfType<HideEnumValueAttribute>());
            var stringValues = values.Select(x => x.ToString() as object).ToList();
           // var objValues = stringValues.Cast<object>().ToList();
            
           
            var list = stringValues.ToList();
            result.Values = list.Paginate(pageSize, page);
            result.TotalCount = list.Count;
            return result;
        }

        var query = GenerateBaseQueryable(dbSet, filters);
        IQueryable<object> query2;

        // check for ICollection<>

        var property = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute);
        
        if (propertyType.IsIEnumerable())
        {
            query2 = (IQueryable<object>)query.Select(property).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(property);
        }

        var converter = mappedToPropertyAttribute.Encrypted
            ? Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(EncryptedConverter))
            : Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(DirectConverter));

        var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertFrom");

        IQueryable<object> query3;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : query3.LongCount();
            result.Values = query3.Skip(pageSize * (page - 1)).Take(pageSize)
                .ToList()
                .Select(x => method.Invoke(converter, [x])!).ToList();
            return result;
        }
        catch
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : long.MaxValue;
            result.Values = query3.Skip(pageSize * (page - 1)).Take(pageSize)
                .ToList()
                .Select(x => method.Invoke(converter, [x])!).ToList();
            return result;
        }
    }

    public static async Task<DistinctColumnValuesResult> DistinctColumnValuesAsync<TModel>(
        this IQueryable<TModel> dbSet,
        List<FilterDto> filters,
        string columnName, int pageSize, int page, CancellationToken cancellationToken = default) where TModel : class
    {
        var result = new DistinctColumnValuesResult();

        var targetProperty = typeof(TModel).GetTargetType().GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .FirstOrDefault(x => x.Name == columnName);

        if (targetProperty is null)
            throw new PropertyNotFoundException($"Property {columnName} not found in {typeof(TModel).Name}");

        var mappedToPropertyAttribute = targetProperty.GetCustomAttribute<MappedToPropertyAttribute>()!;
        if (mappedToPropertyAttribute.Encrypted)
            return new();

        var propertyType = PropertyHelper.GetPropertyType(typeof(TModel), mappedToPropertyAttribute);

        if (propertyType.EnumCheck())
        {
            var values =  Enum.GetValues(GetEnumerableType(propertyType)).Cast<object>().Where(x => !(x as Enum)!.HasAttributeOfType<HideEnumValueAttribute>());
            var stringValues = values.Select(x => x.ToString() as object).ToList();
           // var objValues = stringValues.Cast<object>().ToList();
           
            var list = stringValues.ToList();
            result.Values = list.Paginate(pageSize, page);
            result.TotalCount = list.Count;
            return result;
        }

        var query = GenerateBaseQueryable(dbSet, filters);
        IQueryable<object> query2;

        // check for ICollection<>

        var property = PropertyHelper.GetPropertyLambda(mappedToPropertyAttribute);
        
        if (propertyType.IsIEnumerable())
        {
            query2 = (IQueryable<object>)query.Select(property).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(property);
        }

        var converter = mappedToPropertyAttribute.Encrypted
            ? Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(EncryptedConverter))
            : Activator.CreateInstance(mappedToPropertyAttribute.ConverterType ?? typeof(DirectConverter));

        var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertFrom");

        IQueryable<object> query3;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : query3.LongCount();
            result.Values = (await  query3.Skip(pageSize * (page - 1)).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken))
                .Select(x => method.Invoke(converter, [x])!).ToList();
            return result;
        }
        catch
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = mappedToPropertyAttribute.Encrypted ? 1 : long.MaxValue;
            result.Values =(await query3.Skip(pageSize * (page - 1)).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken))
                .Select(x => method.Invoke(converter, [x])!).ToList();
            return result;
        }
    }
}