using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;
using PandaTech.IEnumerableFilters.Helpers;

namespace PandaTech.IEnumerableFilters;

public static class EnumerableExtendersV3
{
    public static IQueryable<TModel> ApplyFilters<TModel, TDto>(this IQueryable<TModel> dbSet, List<FilterDto> filters)
    {
        var q = dbSet;

        var dtoType = typeof(TDto);
        var mappedToClassAttribute = dtoType.GetCustomAttribute<MappedToClassAttribute>();
        if (mappedToClassAttribute is null)
            throw new MappingException($"Dto {dtoType.Name} is not mapped to any class");
        if (mappedToClassAttribute.TargetType != typeof(TModel))
            throw new MappingException($"Dto {dtoType.Name} is not mapped to {typeof(TModel).Name}");

        var mappedProperties = dtoType.GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .Select(x => new
            {
                x.Name,
                Attribute = x.GetCustomAttribute<MappedToPropertyAttribute>()!,
                Type = x.PropertyType
            }).ToDictionary(x => x.Name, x => new { x.Attribute, x.Type });

        var violations = filters.Where(x => !mappedProperties.ContainsKey(x.PropertyName))
            .Select(x => "Property " + x.PropertyName + " not mapped").ToList();

        if (violations.Any())
            throw new MappingException(string.Join("\n", violations));

        foreach (var filterDto in filters)
        {
            var filter = mappedProperties[filterDto.PropertyName];

            var targetProperty = typeof(TModel).GetProperty(filter.Attribute.TargetPropertyName);
            if (targetProperty is null)
                throw new PropertyNotFoundException(
                    $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(TModel).Name}");

            var filterType = filter.Attribute.FilterType ?? filter.Type;
            var filterTypeName = filterType.Name;

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                var val = (JsonElement)filterDto.Values[index];

                if (filterType.IsEnum)
                {
                    var enumType = filterType;
                    var getExpression = Expression.Call(typeof(Enum), "Parse", null,
                        Expression.Constant(enumType), Expression.Constant(val.GetString()!));

                    var lambda = Expression.Lambda<Func<object>>(getExpression).Compile();

                    filterDto.Values[index] = lambda();
                }
                else if (filterType.Name == "List`1")
                {
                    filterDto.Values[index] =
                        val.ValueKind == JsonValueKind.String ? val.GetString()! : val.GetInt32();
                }
                else
                {
                    filterDto.Values[index] = filterTypeName switch
                    {
                        "String" => val.GetString()!,
                        "Int32" => val.GetInt32(),
                        "Int64" => val.GetInt64(),
                        "Boolean" => val.GetBoolean(),
                        "DateTime" => val.GetDateTime(),
                        "Decimal" => val.GetDecimal(),
                        "Double" => val.GetDouble(),
                        "Single" => val.GetSingle(),
                        "Guid" => val.GetGuid(),
                        _ => Activator.CreateInstance(filterType)!
                    };
                }
            }

            var converter =
                Activator.CreateInstance(filter.Attribute.TargetConverterType ?? typeof(DirectConverter));


            var finalLambda = FilterProvider.BuildLambdaString(new FilterProvider.FilterKey
            {
                ComparisonType = filterDto.ComparisonType,
                TargetPropertyType = targetProperty.PropertyType,
                TargetPropertyName = targetProperty.Name
            });

            var method = converter!.GetType().GetMethods().First(x => x.Name == "Convert");

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                filterDto.Values[index] = method.Invoke(converter, new[] { filterDto.Values[index] }) ??
                                          throw new MappingException("Converter returned null");
            }

            q = filterDto.ComparisonType switch
            {
                ComparisonType.Between => q.Where(finalLambda, filterDto.Values[0], filterDto.Values[1]),
                ComparisonType.In => q.Where(finalLambda, filterDto.Values),
                ComparisonType.Contains when filter.Type != typeof(string) => q.Where(finalLambda,
                    filterDto.Values[0]),
                _ => q.Where(finalLambda, filterDto.Values[0])
            };
        }

        return q;
    }

    public static IQueryable<TModel> ApplyOrdering<TModel, TDto>(this IEnumerable<TModel> dbSet, Ordering ordering)
    {
        if (ordering.PropertyName == string.Empty)
            return dbSet.AsQueryable();

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
            ? dbSet.AsQueryable().OrderBy(filter.TargetPropertyName)
            : dbSet.AsQueryable().OrderBy(filter.TargetPropertyName + " DESC");
    }

    private class FilteringInfo
    {
        
    }
    // TODO: add async version
    public static List<object> DistinctColumnValues<T, TDto>(this IQueryable<T> dbSet, List<FilterDto> filters,
        string columnName, int pageSize, int page, out long totalCount) where T : class
    {
        var mappedProperties = typeof(TDto).GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .Select(x => new
            {
                x.Name,
                Attribute = x.GetCustomAttribute<MappedToPropertyAttribute>()!,
                Type = x.PropertyType
            }).ToDictionary(x => x.Name, x => new { x.Attribute, x.Type });


        var filter = mappedProperties[columnName];

        var targetProperty = typeof(T).GetProperty(filter.Attribute.TargetPropertyName);
        if (targetProperty is null)
            throw new PropertyNotFoundException(
                $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(T).Name}");
        var propertyType = targetProperty.PropertyType;

        // same for list 
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (propertyType.GetGenericArguments()[0].IsEnum)
            {
                totalCount = Enum.GetValues(propertyType.GetGenericArguments()[0]).Length;
                var list = Enum.GetValues(propertyType.GetGenericArguments()[0]).Cast<object>().ToList();
                return list.Where(x => !(x as Enum)!.HasAttributeOfType<HideEnumValueAttribute>()).ToList();
            }
        }


        var query = dbSet.ApplyFilters<T, TDto>(filters);
        IQueryable<object> query2;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            query2 = (IQueryable<object>)query.Select(filter.Attribute.TargetPropertyName).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(filter.Attribute.TargetPropertyName);
        }

        var converter = Activator.CreateInstance(filter.Attribute.TargetConverterType ?? typeof(DirectConverter));
        var method = converter!.GetType().GetMethods().First(x => x.Name == "Convert");

        IQueryable<object> query3;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            totalCount = query3.Count();
            return query3.Skip(pageSize * (page - 1)).Take(pageSize).ToList()
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
        }
        catch
        {
            query3 = query2;
            totalCount = long.MaxValue;
            return query3.Skip(pageSize * (page - 1)).Take(pageSize * 10).Distinct().AsEnumerable()
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
        }
    }
    
    public static async Task<DistinctColumnValuesResult> DistinctColumnValuesAsync<T, TDto>(this IQueryable<T> dbSet, List<FilterDto> filters,
        string columnName, int pageSize, int page, CancellationToken cancellationToken = default) where T : class
    {
        var result = new DistinctColumnValuesResult();
        
        var mappedProperties = typeof(TDto).GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .Select(x => new
            {
                x.Name,
                Attribute = x.GetCustomAttribute<MappedToPropertyAttribute>()!,
                Type = x.PropertyType
            }).ToDictionary(x => x.Name, x => new { x.Attribute, x.Type });
        
     var filter = mappedProperties[columnName];

        var targetProperty = typeof(T).GetProperty(filter.Attribute.TargetPropertyName);
        if (targetProperty is null)
            throw new PropertyNotFoundException(
                $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(T).Name}");
        var propertyType = targetProperty.PropertyType;

        // same for list 
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (propertyType.GetGenericArguments()[0].IsEnum)
            {
                var list = Enum.GetValues(propertyType.GetGenericArguments()[0]).Cast<object>().ToList();
                result.Values = list.Where(x => !(x as Enum)!.HasAttributeOfType<HideEnumValueAttribute>()).ToList();
                result.TotalCount = result.Values.Count;
                 return result;
            }
        }


        var query = dbSet.ApplyFilters<T, TDto>(filters);
        IQueryable<object> query2;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            query2 = (IQueryable<object>)query.Select(filter.Attribute.TargetPropertyName).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(filter.Attribute.TargetPropertyName);
        }

        var converter = Activator.CreateInstance(filter.Attribute.TargetConverterType ?? typeof(DirectConverter));
        var method = converter!.GetType().GetMethods().First(x => x.Name == "Convert");

        IQueryable<object> query3;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = await query3.CountAsync(cancellationToken);
            result.Values = (await query3.Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync(cancellationToken: cancellationToken))
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
            return result;
        }
        catch
        {
            query3 = query2;
            result.TotalCount  = long.MaxValue;
            result.Values = (await query3.Skip(pageSize * (page - 1)).Take(pageSize * 10).Distinct().ToListAsync(cancellationToken: cancellationToken))
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
            return result;
        }
    }   
    
    
}

public struct DistinctColumnValuesResult
{
    public List<object> Values;
    public long TotalCount;
}