using System.ComponentModel;
using System.Linq.Expressions;
using System.Text.Json;
using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters;

public static class EnumerableExtenders
{
    public static IQueryable<T> ApplyFilters<T>(this IEnumerable<T> dbSet, List<FilterDto> filters,
        FilterProvider filterProvider)
        where T : class
    {
        try
        {
            foreach (var filterDto in filters)
            {
                var filter = filterProvider.GetFilter(filterDto.PropertyName, filterDto.ComparisonType);

                var property = typeof(T).GetProperty(filterDto.PropertyName)!;

                var filterType = filter?.FilterType ?? property.PropertyType;
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
                    // is list
                    else if (filterType.Name == "List`1")
                    {
                        filterDto.Values[index] =
                            val.ValueKind == JsonValueKind.String ? val.GetString()! : val.GetInt64();
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
                        };
                    }
                }


                filterDto.FilterOverride = filter;

                for (var index = 0; index < filterDto.Values.Count; index++)
                {
                    filterDto.Values[index] = filter?.Converter(filterDto.Values[index]) ?? filterDto.Values[index];
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return dbSet.ApplyFilters(filters);
    }

    private static readonly List<ComparisonType> SingleParameterComparisonTypes = new List<ComparisonType>
    {
        ComparisonType.Equal, ComparisonType.EndsWith, ComparisonType.GreaterThan,
        ComparisonType.GreaterThanOrEqual,
        ComparisonType.LessThan, ComparisonType.LessThanOrEqual, ComparisonType.NotEqual,
        ComparisonType.StartsWith, ComparisonType.Contains, ComparisonType.HasCountEqualTo
    };

    private static readonly List<ComparisonType> DoubleParameterComparisonTypes = new List<ComparisonType>
    {
        ComparisonType.Between
    };

    private static readonly List<ComparisonType> ListParameterComparisonTypes = new List<ComparisonType>
    {
        ComparisonType.In, ComparisonType.NotIn
    };


    private static IQueryable<T> ApplyFilters<T>(this IEnumerable<T> dbSet, List<FilterDto> filters)
        where T : class
    {
        var query = dbSet.AsQueryable();


        foreach (var filter in filters)
        {
            var property = typeof(T).GetProperty(filter.PropertyName);
            
            var propertyType = filter.FilterOverride?.TargetPropertyType ?? property?.PropertyType ??
                throw new PropertyNotFoundException($"Property {filter.PropertyName} not found");

            if (filter.FilterOverride == null)
                if (ComparisonTypes.TryGetValue(propertyType.Name, out var comparisonTypes))
                {
                    if (!comparisonTypes.Contains(filter.ComparisonType))
                        throw new ComparisonNotSupportedException("Comparison type not supported for this property");
                }
                else
                    throw new ComparisonNotSupportedException("Comparison type not supported for this property");

            var parameter = Expression.Parameter(typeof(T));
            var propertyValue = filter.FilterOverride?.SourcePropertyConverter ??
                                Expression.Property(parameter, filter.PropertyName);
            /*var listConstant = Expression.Constant(filter.Values
                .Select(x => x).ToList());
                */
            
            /*if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyType = propertyType.GetGenericArguments()[0];
                propertyValue = Expression.Property(propertyValue, "Value");
            }*/
            
            var listType = typeof(List<>).MakeGenericType(propertyType);

            var newList = Activator.CreateInstance(listType); 
            var addMethod = listType.GetMethod("Add")!;
            foreach (var value in filter.Values)
            {
                addMethod.Invoke(newList, new[] { value });
            }
            
            var listConstant = Expression.Constant(newList, listType);
            
            Expression<Func<T, bool>>? lambda = null;
            if (propertyType.IsEnum)
            {
                Expression expression;
                switch (filter.ComparisonType)
                {
                    case ComparisonType.Equal:
                        expression = Expression.Equal(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.NotEqual:
                        expression = Expression.NotEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.In:
                        expression = Expression.Call(listConstant,
                            typeof(List<object>).GetMethod("Contains", new[] { typeof(object) })!,
                            propertyValue);
                        break;
                    case ComparisonType.NotIn:
                        expression = Expression.Not(Expression.Call(listConstant,
                            typeof(List<object>).GetMethod("Contains", new[] { typeof(object) })!,
                            propertyValue));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType == typeof(string))
            {
                Expression expression;
                switch (filter.ComparisonType)
                {
                    case ComparisonType.Contains:
                        expression = Expression.Call(
                            propertyValue,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.StartsWith:
                        expression = Expression.Call(propertyValue,
                            typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.EndsWith:
                        expression = Expression.Call(propertyValue,
                            typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.Equal:
                        expression = Expression.Equal(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.NotEqual:
                        expression = Expression.NotEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.In:
                        expression = Expression.Call(listConstant,
                            typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!, propertyValue);
                        break;
                    case ComparisonType.NotIn:
                        expression = Expression.Not(Expression.Call(listConstant,
                            typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!, propertyValue));
                        break;
                    case ComparisonType.NotContains:
                        expression = Expression.Not(Expression.Call(propertyValue,
                            typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                            Expression.Constant(filter.Values.First())));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType == typeof(int))
            {
                Expression expression;
                switch (filter.ComparisonType)
                {
                    case ComparisonType.Equal:
                        expression = Expression.Equal(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.NotEqual:
                        expression = Expression.NotEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThan:
                        expression = Expression.GreaterThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThan:
                        expression = Expression.LessThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.In:
                        expression = Expression.Call(listConstant,
                            typeof(List<int>).GetMethod("Contains", new[] { typeof(int) })!, propertyValue);
                        break;
                    case ComparisonType.NotIn:
                        expression = Expression.Not(Expression.Call(listConstant,
                            typeof(List<int>).GetMethod("Contains", new[] { typeof(int) })!, propertyValue));
                        break;
                    case ComparisonType.Between:
                        var lowerBound = Expression.Constant(filter.Values.First());
                        var upperBound = Expression.Constant(filter.Values[1]);
                        expression = Expression.And(Expression.GreaterThanOrEqual(propertyValue, lowerBound),
                            Expression.LessThanOrEqual(propertyValue, upperBound));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType == typeof(long))
            {
                Expression expression;
                switch (filter.ComparisonType)
                {
                    case ComparisonType.Equal:
                        expression = Expression.Equal(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.NotEqual:
                        expression = Expression.NotEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThan:
                        expression = Expression.GreaterThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThan:
                        expression = Expression.LessThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.In:
                        expression = Expression.Call(listConstant,
                            typeof(List<long>).GetMethod("Contains", new[] { typeof(long) })!, propertyValue);
                        break;
                    case ComparisonType.NotIn:
                        expression = Expression.Not(Expression.Call(listConstant,
                            typeof(List<long>).GetMethod("Contains", new[] { typeof(long) })!, propertyValue));
                        break;
                    case ComparisonType.Between:
                        var lowerBound = Expression.Constant(filter.Values.First());
                        var upperBound = Expression.Constant(filter.Values[1]);
                        expression = Expression.And(Expression.GreaterThanOrEqual(propertyValue, lowerBound),
                            Expression.LessThanOrEqual(propertyValue, upperBound));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType == typeof(DateTime))
            {
                Expression expression;
                switch (filter.ComparisonType)
                {
                    case ComparisonType.Equal:
                        expression = Expression.Equal(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.NotEqual:
                        expression = Expression.NotEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThan:
                        expression = Expression.GreaterThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThan:
                        expression = Expression.LessThan(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(propertyValue,
                            Expression.Constant(filter.Values.First()));
                        break;
                    case ComparisonType.In:
                        expression = Expression.Call(listConstant,
                            typeof(List<DateTime>).GetMethod("Contains", new[] { typeof(DateTime) })!, propertyValue);
                        break;
                    case ComparisonType.NotIn:
                        expression = Expression.Not(Expression.Call(listConstant,
                            typeof(List<DateTime>).GetMethod("Contains", new[] { typeof(DateTime) })!, propertyValue));
                        break;
                    case ComparisonType.Between:
                        var lowerBound = Expression.Constant(filter.Values.First());
                        var upperBound = Expression.Constant(filter.Values[1]);
                        expression = Expression.And(Expression.GreaterThanOrEqual(propertyValue, lowerBound),
                            Expression.LessThanOrEqual(propertyValue, upperBound));
                        break;
                    default: throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType == typeof(bool))
            {
                var boolConstant = Expression.Constant(true);

                Expression expression = filter.ComparisonType switch
                {
                    ComparisonType.IsTrue => Expression.Equal(propertyValue, boolConstant),
                    ComparisonType.IsFalse => Expression.NotEqual(propertyValue, boolConstant),
                    _ => throw new NotImplementedException()
                };

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            if (propertyType.Name == "List`1")
            {
                Expression expression;

                switch (filter.ComparisonType)
                {
                    case ComparisonType.HasCountEqualTo:
                        var count = Expression.Property(propertyValue, "Count");
                        var zero = Expression.Constant(filter.Values.First());
                        expression = Expression.Equal(count, zero);
                        break;
                    case ComparisonType.HasCountBetween:
                        var count1 = Expression.Property(propertyValue, "Count");
                        var zero1 = Expression.Constant(filter.Values.First());
                        var one = Expression.Constant(filter.Values[1]);
                        expression = Expression.And(Expression.GreaterThanOrEqual(count1, zero1),
                            Expression.LessThanOrEqual(count1, one));
                        break;
                    case ComparisonType.IsEmpty:
                        var count2 = Expression.Property(propertyValue, "Count");
                        var zero2 = Expression.Constant(0);
                        expression = Expression.Equal(count2, zero2);
                        break;
                    case ComparisonType.IsNotEmpty:
                        var count3 = Expression.Property(propertyValue, "Count");
                        var zero3 = Expression.Constant(0);
                        expression = Expression.NotEqual(count3, zero3);
                        break;
                    case ComparisonType.Contains:
                        expression = Expression.Call(listConstant,
                            typeof(List<int>).GetMethod("Contains", new[] { typeof(int) })!, propertyValue);
                        break;
                    default: throw new NotImplementedException();
                }

                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }

            /*if (propertyType == typeof(Guid))
                throw new NotImplementedException();
                */

            if (propertyType.IsClass && propertyType != typeof(string) && propertyType != typeof(DateTime))
            {
                Expression expression = filter.ComparisonType switch
                {
                    ComparisonType.Equal => Expression.Equal(propertyValue, Expression.Constant(filter.Values.First())),
                    _ => throw new NotImplementedException()
                };
                lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
            }


            if (lambda is null)
                continue;
            query = query.Where(lambda);
        }

        return query;
    }

    public static IQueryable<T> ApplyOrdering<T>(this IEnumerable<T> dbSet, Ordering ordering)
    {
        if (ordering.PropertyName == string.Empty)
            return dbSet.AsQueryable();

        var property = typeof(T).GetProperty(ordering.PropertyName);
        if (property is null)
            throw new Exception("Column not found");

        var parameter = Expression.Parameter(typeof(T));
        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda<Func<T, object>>(Expression.Convert(propertyAccess, typeof(object)),
            parameter);

        return !ordering.Descending
            ? dbSet.AsQueryable().OrderBy(lambda)
            : dbSet.AsQueryable().OrderByDescending(lambda);
    }


    public static Dictionary<string, object?> GetAggregates<T>(this IEnumerable<T> dbSet, List<AggregateDto> aggregates)
        where T : class
    {
        var query = dbSet.AsQueryable();

        var result = new Dictionary<string, object?>();
        foreach (var aggregate in aggregates)
        {
            var property = typeof(T).GetProperty(aggregate.PropertyName);

            if (property is null)
                throw new Exception("Column not found");
            var parameter = Expression.Parameter(typeof(T));
            var propertyAccess = Expression.Property(parameter, property);


            if (property.PropertyType == typeof(string))
            {
                var lambda = Expression.Lambda<Func<T, string>>(propertyAccess, parameter);

                decimal? res = aggregate.AggregateType switch
                {
                    AggregateType.UniqueCount => query.Select(lambda).Distinct().ToList().Count,
                    _ => null
                };

                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(int))
            {
                var lambda = Expression.Lambda<Func<T, int>>(propertyAccess, parameter);

                double? res = aggregate.AggregateType switch
                {
                    AggregateType.UniqueCount => query.Select(lambda).Distinct().ToList().Count,
                    AggregateType.Sum => query.Select(lambda).Sum(),
                    AggregateType.Average => query.Select(lambda).Average(),
                    AggregateType.Min => query.Select(lambda).Min(),
                    AggregateType.Max => query.Select(lambda).Max(),
                    _ => null
                };

                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(long))
            {
                var lambda = Expression.Lambda<Func<T, long>>(propertyAccess, parameter);

                double? res = aggregate.AggregateType switch
                {
                    AggregateType.UniqueCount => query.Select(lambda).Distinct().ToList().Count,
                    AggregateType.Sum => query.Select(lambda).Sum(),
                    AggregateType.Average => query.Select(lambda).Average(),
                    AggregateType.Min => query.Select(lambda).Min(),
                    AggregateType.Max => query.Select(lambda).Max(),
                    _ => null
                };

                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(DateTime))
            {
                var lambda = Expression.Lambda<Func<T, DateTime>>(propertyAccess, parameter);
                DateTime? res = aggregate.AggregateType switch
                {
                    AggregateType.Min => query.Select(lambda).Min(),
                    AggregateType.Max => query.Select(lambda).Max(),
                    AggregateType.Average => new DateTime(
                        Convert.ToInt64(query.Select(lambda).ToList().Average(x => x.Ticks / 10_000_000)) * 10_000_000),
                    _ => null
                };
                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(decimal))
            {
                var lambda = Expression.Lambda<Func<T, decimal>>(propertyAccess, parameter);

                decimal? res = aggregate.AggregateType switch
                {
                    AggregateType.UniqueCount => query.Select(lambda).Distinct().ToList().Count,
                    AggregateType.Sum => query.Select(lambda).Sum(),
                    AggregateType.Average => query.Select(lambda).Average(),
                    AggregateType.Min => query.Select(lambda).Min(),
                    AggregateType.Max => query.Select(lambda).Max(),
                    _ => null
                };

                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(double))
            {
                var lambda = Expression.Lambda<Func<T, double>>(propertyAccess, parameter);

                double? res = aggregate.AggregateType switch
                {
                    AggregateType.UniqueCount => query.Select(lambda).Distinct().ToList().Count,
                    AggregateType.Sum => query.Select(lambda).Sum(),
                    AggregateType.Average => query.Select(lambda).Average(),
                    AggregateType.Min => query.Select(lambda).Min(),
                    AggregateType.Max => query.Select(lambda).Max(),
                    _ => null
                };

                result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", res);
                continue;
            }

            if (property.PropertyType == typeof(Guid))
            {
                throw new NotImplementedException();
            }

            if (property.PropertyType.IsClass)
            {
                throw new NotImplementedException();
            }

            result.Add($"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}", null);
        }

        return result;
    }

    public static List<string> DistinctColumnValues<T>(this IEnumerable<T> dbSet, List<FilterDto> filters,
        string columnName,
        int pageSize, int page, out long totalCount) where T : class
    {
        var property = typeof(T).GetProperty(columnName);

        if (property is null)
            throw new Exception("Column not found");
        var parameter = Expression.Parameter(typeof(T));
        var propertyAccess = Expression.Property(parameter, property);
        var propertyType = property.PropertyType;
        //add cast to string

        Expression<Func<T, string>> lambda;
        if (propertyType != typeof(string))
        {
            var convert = Expression.Call(propertyAccess, "ToString", null, null);
            lambda = Expression.Lambda<Func<T, string>>(convert, parameter);
        }
        else
        {
            lambda = Expression.Lambda<Func<T, string>>(propertyAccess, parameter);
        }

        var query = dbSet.ApplyFilters(filters).Select(lambda).Distinct().OrderBy(x => x);

        totalCount = query.LongCount();
        return query.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
    }

    public static readonly Dictionary<string, List<ComparisonType>> ComparisonTypes = new()
    {
        {
            "String",
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.Contains, ComparisonType.StartsWith, ComparisonType.EndsWith,
                ComparisonType.In, ComparisonType.NotIn, ComparisonType.NotEqual, ComparisonType.NotContains
            }
        },
        {
            "Int32",
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.GreaterThan, ComparisonType.GreaterThanOrEqual,
                ComparisonType.LessThan, ComparisonType.LessThanOrEqual, ComparisonType.In, ComparisonType.NotIn,
                ComparisonType.Between, ComparisonType.NotEqual
            }
        },
        {
            "Int64",
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.GreaterThan, ComparisonType.GreaterThanOrEqual,
                ComparisonType.LessThan, ComparisonType.LessThanOrEqual, ComparisonType.In, ComparisonType.NotIn,
                ComparisonType.Between, ComparisonType.NotEqual
            }
        },
        {
            nameof(Decimal),
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.GreaterThan, ComparisonType.GreaterThanOrEqual,
                ComparisonType.LessThan, ComparisonType.LessThanOrEqual, ComparisonType.In, ComparisonType.NotIn,
                ComparisonType.Between, ComparisonType.NotEqual
            }
        },
        {
            "DateTime",
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.GreaterThan, ComparisonType.GreaterThanOrEqual,
                ComparisonType.LessThan, ComparisonType.LessThanOrEqual, ComparisonType.In, ComparisonType.NotIn,
                ComparisonType.Between, ComparisonType.NotEqual
            }
        },
        {
            "Boolean",
            new List<ComparisonType>
            {
                ComparisonType.IsTrue, ComparisonType.IsFalse
            }
        },
        {
            "List`1",
            new List<ComparisonType>
            {
                ComparisonType.HasCountEqualTo, ComparisonType.HasCountBetween, ComparisonType.Contains,
                ComparisonType.NotContains, ComparisonType.Contains
            }
        },
        {
            "Guid",
            new List<ComparisonType>
            {
                ComparisonType.Equal, ComparisonType.NotEqual, ComparisonType.In, ComparisonType.NotIn
            }
        },
        {
            "Class", new List<ComparisonType>
            {
                ComparisonType.In, ComparisonType.NotIn, ComparisonType.Equal, ComparisonType.NotEqual
            }
        },
        {
            "Enum", new List<ComparisonType>
            {
                ComparisonType.In, ComparisonType.NotIn, ComparisonType.Equal, ComparisonType.NotEqual
            }
        }
    };
}

public class ComparisonNotSupportedException : Exception
{
    public ComparisonNotSupportedException(string message) : base(message)
    {
    }
}

public class PropertyNotFoundException : Exception
{
    public PropertyNotFoundException(string message) : base(message)
    {
    }
}