using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;
using PandaTech.IEnumerableFilters.Helpers;
using PandaTech.IEnumerableFilters.PostgresContext;
using static System.Linq.Expressions.Expression;

namespace PandaTech.IEnumerableFilters;

public static class EnumerableExtendersV3
{
    private static ComparisonType[]? ComparisonTypes(ComparisonTypesDefault typesDefault) =>
        typesDefault switch
        {
            ComparisonTypesDefault.Numeric => DefaultComparisonTypes.Numeric,
            ComparisonTypesDefault.String => DefaultComparisonTypes.String,
            ComparisonTypesDefault.DateTime => DefaultComparisonTypes.DateTime,
            ComparisonTypesDefault.Bool => DefaultComparisonTypes.Bool,
            ComparisonTypesDefault.Guid => DefaultComparisonTypes.Guid,
            ComparisonTypesDefault.Enum => DefaultComparisonTypes.Enum,
            ComparisonTypesDefault.ByteArray => DefaultComparisonTypes.ByteArray,
            _ => null
        };

    private static ComparisonTypesDefault GetComparisonTypesDefault(Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            type = type.GetGenericArguments()[0];

        if (type == typeof(int) || type == typeof(long) || type == typeof(decimal) || type == typeof(double) ||
            type == typeof(float))
            return ComparisonTypesDefault.Numeric;
        if (type == typeof(string))
            return ComparisonTypesDefault.String;
        if (type == typeof(DateTime))
            return ComparisonTypesDefault.DateTime;
        if (type == typeof(bool))
            return ComparisonTypesDefault.Bool;
        if (type == typeof(Guid))
            return ComparisonTypesDefault.Guid;
        if (type.IsEnum)
            return ComparisonTypesDefault.Enum;
        if (type == typeof(byte[]))
            return ComparisonTypesDefault.ByteArray;
        throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }

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

            var filterType = filter.Attribute.ConverterType is not null
                ? filter.Attribute.ConverterType.GenericTypeArguments.First()
                : filter.Type;

            var filterTypeName = filterType.Name;

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                var val = (JsonElement)filterDto.Values[index];

                if (filterType.IsEnum)
                {
                    var enumType = filterType;
                    var getExpression = Call(typeof(Enum), "Parse", null,
                        Constant(enumType), Constant(val.GetString()!));

                    var lambda = Lambda<Func<object>>(getExpression).Compile();

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
                        "String" => filter.Attribute.Encrypted ? val.GetString()! : val.GetString()!.ToLower(),
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


            if (filter.Attribute.Encrypted)
            {
                if (!new[] { ComparisonType.Equal, ComparisonType.In, ComparisonType.NotIn, ComparisonType.NotEqual }
                        .Contains(filterDto.ComparisonType))
                    throw new ComparisonNotSupportedException(
                        $"Comparison type {filterDto.ComparisonType} not supported for encrypted property");

                if (filterDto.ComparisonType != ComparisonType.Equal &&
                    filterDto.ComparisonType != ComparisonType.In
                   )
                    throw new ComparisonNotSupportedException(
                        $"Comparison type {filterDto.ComparisonType} not supported for encrypted property");


                // var hashes = filterDto.Values.Select(x => Pandatech.Crypto.Sha3.Hash(x.ToString()!)).ToList();
                //.Where(x => hashes.Contains(PostgresDbContext.substr(x.SomeBytes, 1, 64)));

                var parameter = Parameter(typeof(TModel));
                var property = Property(parameter, targetProperty);

                var hashes = filterDto.Values.Select(x => Pandatech.Crypto.Sha3.Hash(x.ToString()!)).ToList();

                var substrMethod =
                    typeof(PostgresDbContext).GetMethod("substr", new[] { typeof(byte[]), typeof(int), typeof(int) });

                var substrExpression = Call(substrMethod!, property, Constant(1), Constant(64));

                var containsMethod = typeof(List<byte[]>).GetMethod("Contains", new[] { typeof(byte[]) });

                var containsExpression = Call(Constant(hashes), containsMethod!, substrExpression);

                var lambda = new[] { ComparisonType.NotIn, ComparisonType.NotEqual }.Contains(filterDto.ComparisonType)
                    ? Lambda<Func<TModel, bool>>(Not(containsExpression), parameter)
                    : Lambda<Func<TModel, bool>>(containsExpression, parameter);

                q = q.Where(lambda);

                return q;
            }


            var converter =
                Activator.CreateInstance(filter.Attribute.ConverterType ?? typeof(DirectConverter));

            Type targetPropertyType;
            if (filter.Attribute.SubPropertyRoute == "")
            {
                targetPropertyType = targetProperty.PropertyType;
            }
            else
            {
                var subProperty = targetProperty.PropertyType.GetProperty(filter.Attribute.SubPropertyRoute);
                if (subProperty is null)
                    throw new PropertyNotFoundException(
                        $"Property {filter.Attribute.SubPropertyRoute} not found in {targetProperty.Name}");
                targetPropertyType = subProperty.PropertyType;
            }

            var finalLambda = FilterLambdaBuilder.BuildLambdaString(new FilterKey
            {
                ComparisonType = filterDto.ComparisonType,
                TargetPropertyType = targetPropertyType,
                TargetPropertyName = targetProperty.Name + (filter.Attribute.SubPropertyRoute == ""
                    ? ""
                    : "." + filter.Attribute.SubPropertyRoute),
            });

            var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertTo");

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                filterDto.Values[index] = method.Invoke(converter, new[] { filterDto.Values[index] }) ??
                                          throw new MappingException("Converter returned null");
            }

            var typedList = Activator.CreateInstance(typeof(List<>).MakeGenericType(filterType));

            var addMethod = typedList!.GetType().GetMethod("Add");
            foreach (var value in filterDto.Values)
            {
                addMethod!.Invoke(typedList, new[] { value });
            }

            q = filterDto.ComparisonType switch
            {
                ComparisonType.Between => q.Where(finalLambda, filterDto.Values[0], filterDto.Values[1]),
                ComparisonType.In => q.Where(finalLambda, typedList),
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

    public static List<object> DistinctColumnValues<TModel, TDto>(this IQueryable<TModel> dbSet,
        List<FilterDto> filters,
        string columnName, int pageSize, int page, out long totalCount) where TModel : class
    {
        var result = DistinctColumnValues<TModel, TDto>(dbSet, filters, columnName, pageSize, page);

        totalCount = result.TotalCount;
        return result.Values;
    }

    public static DistinctColumnValuesResult DistinctColumnValues<TModel, TDto>(this IQueryable<TModel> dbSet,
        List<FilterDto> filters,
        string columnName, int pageSize, int page) where TModel : class
    {
        var result = new DistinctColumnValuesResult();
        var mappedProperties = typeof(TDto).GetProperties()
            .Where(x => x.GetCustomAttribute<MappedToPropertyAttribute>() != null)
            .Select(x => new
            {
                x.Name,
                Attribute = x.GetCustomAttribute<MappedToPropertyAttribute>()!,
                Type = x.PropertyType,
            }).ToDictionary(x => x.Name, x => new { x.Attribute, x.Type });

        var filter = mappedProperties[columnName];

        if (filter.Attribute.Encrypted && filters.Count == 0)
            return new();

        var targetProperty = typeof(TModel).GetProperty(filter.Attribute.TargetPropertyName);
        if (targetProperty is null)
            throw new PropertyNotFoundException(
                $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(TModel).Name}");
        var propertyType = targetProperty.PropertyType;

        // same for list 
        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            if (propertyType.GetGenericArguments()[0].IsEnum)
            {
                var list = Enum.GetValues(propertyType.GetGenericArguments()[0]).Cast<object>().ToList();
                result.Values = list;
                result.TotalCount = list.Count;
                return result;
            }
        }

        var query = dbSet.ApplyFilters<TModel, TDto>(filters);
        IQueryable<object> query2;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            query2 = (IQueryable<object>)query.Select(filter.Attribute.TargetPropertyName).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(filter.Attribute.TargetPropertyName);
        }

        var converter = Activator.CreateInstance(filter.Attribute.ConverterType ?? typeof(DirectConverter));
        var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertFrom");

        IQueryable<object> query3;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = query3.LongCount();
            result.Values = (query3.Skip(pageSize * (page - 1)).Take(pageSize)
                    .ToList())
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
            return result;
        }
        catch
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = long.MaxValue;
            result.Values = (query3.Skip(pageSize * (page - 1)).Take(pageSize)
                    .ToList())
                .Select(x => method.Invoke(converter, new[] { x })!).ToList();
            return result;
        }
    }

    public static async Task<DistinctColumnValuesResult> DistinctColumnValuesAsync<TModel, TDto>(
        this IQueryable<TModel> dbSet,
        List<FilterDto> filters,
        string columnName, int pageSize, int page, CancellationToken cancellationToken = default) where TModel : class
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

        if (filter.Attribute.Encrypted && filters.Count == 0)
            return new();
        
        var targetProperty = typeof(TModel).GetProperty(filter.Attribute.TargetPropertyName);
        if (targetProperty is null)
            throw new PropertyNotFoundException(
                $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(TModel).Name}");
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


        var query = dbSet.ApplyFilters<TModel, TDto>(filters);
        IQueryable<object> query2;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            query2 = (IQueryable<object>)query.Select(filter.Attribute.TargetPropertyName).SelectMany("x => x");
        }
        else
        {
            query2 = query.Select<object>(
                $"x => x.{filter.Attribute.TargetPropertyName}{
                    (filter.Attribute.SubPropertyRoute == "" ? "" : "." + filter.Attribute.SubPropertyRoute)}");
        }

        var converter = filter.Attribute.Encrypted
            ? Activator.CreateInstance(filter.Attribute.ConverterType ?? typeof(EncryptedConverter))
            : Activator.CreateInstance(filter.Attribute.ConverterType ?? typeof(DirectConverter));
        var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertFrom");

        IQueryable<object> query3;
        List<object> response;
        try
        {
            query3 = query2.Distinct().OrderBy(x => x);
            result.TotalCount = await query3.LongCountAsync(cancellationToken);
            response = await query3.Skip(pageSize * (page - 1)).Take(pageSize)
                .ToListAsync(cancellationToken: cancellationToken);
        }
        catch
        {
            query3 = query2;
            result.TotalCount = long.MaxValue;
            response = await query3.Skip(pageSize * (page - 1)).Take(pageSize * 10).Distinct()
                .ToListAsync(cancellationToken: cancellationToken);
        }

        result.Values = response.Select(x => method.Invoke(converter, new[] { x })!).Distinct().ToList();
        return result;
    }

    public static List<string> GetTables(Assembly assembly)
    {
        return assembly.GetTypes()
            .Where(x => x.CustomAttributes.Any(xx => xx.AttributeType == typeof(MappedToClassAttribute)))
            .Select(x => x.Name)
            .ToList();
    }

    public static List<string> GetTables() => GetTables(Assembly.GetCallingAssembly());

    public static List<FilterInfo> GetFilters(Assembly assembly, string tableName)
    {
        var type = assembly.GetTypes()
            .FirstOrDefault(x => x.Name == tableName &&
                                 x.CustomAttributes.Any(attr =>
                                     attr.AttributeType ==
                                     typeof(MappedToClassAttribute)));

        if (type is null)
            throw new MappingException("Table not found");

        var properties = type.GetProperties().Where(x => x.CustomAttributes.Any(attr =>
            attr.AttributeType == typeof(MappedToPropertyAttribute))).ToList();
        var infos = properties.Select(
            x => new FilterInfo()
            {
                PropertyName = x.Name,
                Table = tableName,
                ComparisonTypes = (x.GetCustomAttribute<MappedToPropertyAttribute>()!.ComparisonTypes ??
                                   ComparisonTypes(GetComparisonTypesDefault(x.PropertyType))
                                   ?? new[]
                                   {
                                       ComparisonType.Equal,
                                       ComparisonType.NotEqual,
                                       ComparisonType.In,
                                       ComparisonType.NotIn
                                   }).ToList()
            }
        );

        return infos.ToList();
    }

    public static List<FilterInfo> GetFilters(string tableName) => GetFilters(Assembly.GetCallingAssembly(), tableName);

    public static async Task<Dictionary<string, object?>> GetAggregatesAsync<TModel, TDto>(
        this IQueryable<TModel> dbSet,
        List<AggregateDto> aggregates, CancellationToken cancellationToken = default) where TModel : class
    {
        List<ImTask> tasks = new();

        foreach (var aggregate in aggregates)
        {
            var sourceProperty = typeof(TDto).GetProperty(aggregate.PropertyName);
            if (sourceProperty is null)
            {
                throw new PropertyNotFoundException(
                    $"Property {aggregate.PropertyName} not found in {typeof(TDto).Name}");
            }

            var attribute = sourceProperty.GetCustomAttribute<MappedToPropertyAttribute>();
            if (attribute is null)
            {
                throw new PropertyNotFoundException(
                    $"Property {aggregate.PropertyName} not mapped in {typeof(TDto).Name}");
            }

            // TODO: Add aggregate availability check in MappedToPropertyAttribute

            var targetProperty = typeof(TModel).GetProperty(attribute.TargetPropertyName);
            if (targetProperty is null)
            {
                throw new PropertyNotFoundException(
                    $"Property {attribute.TargetPropertyName} not found in {typeof(TModel).Name}");
            }

            var parameter = Parameter(typeof(TModel));
            var property = typeof(TModel).GetProperty(aggregate.PropertyName);

            if (property is null)
            {
                throw new Exception("Column not found");
            }

            var prePropertyAccess = Property(parameter, property);

            MemberExpression propertyAccess;
            if (attribute.SubPropertyRoute != "")
            {
                var subProperty = property.PropertyType.GetProperty(attribute.SubPropertyRoute);
                if (subProperty is null)
                {
                    throw new PropertyNotFoundException(
                        $"Property {attribute.SubPropertyRoute} not found in {property.Name}");
                }

                propertyAccess = Property(prePropertyAccess, subProperty);
            }
            else 
            {
                 propertyAccess = prePropertyAccess; //Property(parameter, property);
            }
            

            var key = $"{aggregate.PropertyName}_{aggregate.AggregateType.ToString()}";

            #region type recognition

            if (targetProperty.PropertyType == typeof(string))
            {
                var lambda = Lambda<Func<TModel, string>>(propertyAccess, parameter);

                if (aggregate.AggregateType == AggregateType.UniqueCount)
                {
                    tasks.Add(
                        new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).Distinct()
                                .LongCountAsync(cancellationToken: cancellationToken)
                        }
                    );
                }
                else
                {
                    tasks.Add(
                        new KeyTask<string?>()
                        {
                            Key = key,
                            Task = await Task.FromResult<string?>(null)
                        }
                    );
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(int))
            {
                var lambda = Lambda<Func<TModel, int>>(propertyAccess, parameter);

                switch (aggregate.AggregateType)
                {
                    case AggregateType.UniqueCount:
                    {
                        tasks.Add(new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).Distinct()
                                .LongCountAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Sum:
                    {
                        tasks.Add(new KeyTask<int>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Average:
                    {
                        tasks.Add(new KeyTask<double>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Min:
                    {
                        tasks.Add(new KeyTask<int>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Max:
                    {
                        tasks.Add(new KeyTask<int>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException(paramName: key, message: "Unknown aggregate type");
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(long))
            {
                var lambda = Lambda<Func<TModel, long>>(propertyAccess, parameter);

                switch (aggregate.AggregateType)
                {
                    case AggregateType.UniqueCount:
                    {
                        tasks.Add(new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).Distinct()
                                .LongCountAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Sum:
                    {
                        tasks.Add(new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).SumAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Average:
                    {
                        tasks.Add(new KeyTask<double>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).AverageAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Min:
                    {
                        tasks.Add(new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    case AggregateType.Max:
                    {
                        tasks.Add(new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                        });
                        break;
                    }
                    default: throw new ArgumentOutOfRangeException();
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(DateTime))
            {
                var lambda = Lambda<Func<TModel, DateTime>>(propertyAccess, parameter);
                var res = aggregate.AggregateType switch
                {
                    AggregateType.Min => dbSet.Select(lambda).MinAsync(cancellationToken: cancellationToken),
                    AggregateType.Max => dbSet.Select(lambda).MaxAsync(cancellationToken: cancellationToken),
                    _ => null
                };
                if (res is null)
                {
                    tasks.Add(new KeyTask<DateTime?>()
                    {
                        Key = key,
                        Task = await Task.FromResult<DateTime?>(null)
                    });
                }
                else
                {
                    tasks.Add(new KeyTask<DateTime>()
                    {
                        Key = key,
                        Task = await res
                    });
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(decimal))
            {
                var lambda = Lambda<Func<TModel, decimal>>(propertyAccess, parameter);

                switch (aggregate.AggregateType)
                {
                    case AggregateType.UniqueCount:
                        tasks.Add(
                            new KeyTask<long>()
                            {
                                Task = await dbSet.Select(lambda).Distinct().LongCountAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Sum:
                        tasks.Add(
                            new KeyTask<decimal>()
                            {
                                Task = await dbSet.Select(lambda).SumAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Average:
                        tasks.Add(
                            new KeyTask<decimal>()
                            {
                                Task = await dbSet.Select(lambda).AverageAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Min:
                        tasks.Add(
                            new KeyTask<decimal>()
                            {
                                Task = await dbSet.Select(lambda).MinAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Max:
                        tasks.Add(
                            new KeyTask<decimal>()
                            {
                                Task = await dbSet.Select(lambda).MaxAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    default:
                        tasks.Add(
                            new KeyTask<decimal?>()
                            {
                                Task = await Task.FromResult<decimal?>(null),
                                Key = key
                            });
                        break;
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(double))
            {
                var lambda = Lambda<Func<TModel, double>>(propertyAccess, parameter);

                switch (aggregate.AggregateType)
                {
                    case AggregateType.UniqueCount:
                        tasks.Add(
                            new KeyTask<long>()
                            {
                                Task = await dbSet.Select(lambda).Distinct().LongCountAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Sum:
                        tasks.Add(
                            new KeyTask<double>()
                            {
                                Task = await dbSet.Select(lambda).SumAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Average:
                        tasks.Add(
                            new KeyTask<double>()
                            {
                                Task = await dbSet.Select(lambda).AverageAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Min:
                        tasks.Add(
                            new KeyTask<double>()
                            {
                                Task = await dbSet.Select(lambda).MinAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    case AggregateType.Max:
                        tasks.Add(
                            new KeyTask<double>()
                            {
                                Task = await dbSet.Select(lambda).MaxAsync(cancellationToken),
                                Key = key
                            });
                        break;
                    default:
                        tasks.Add(
                            new KeyTask<double?>()
                            {
                                Task = await Task.FromResult<double?>(null),
                                Key = key
                            });
                        break;
                }

                continue;
            }

            if (targetProperty.PropertyType == typeof(Guid))
            {
                var lambda = Lambda<Func<TModel, Guid>>(propertyAccess, parameter);

                if (aggregate.AggregateType == AggregateType.UniqueCount)
                {
                    tasks.Add(
                        new KeyTask<long>()
                        {
                            Key = key,
                            Task = await dbSet.Select(lambda).Distinct()
                                .LongCountAsync(cancellationToken: cancellationToken)
                        }
                    );
                }
                else
                {
                    tasks.Add(
                        new KeyTask<Guid?>()
                        {
                            Key = key,
                            Task = await Task.FromResult<Guid?>(null)
                        }
                    );
                }
            }

            if (targetProperty.PropertyType.IsClass)
            {
                //TODO: Sub property 

                throw new NotImplementedException();
            }

            if (targetProperty.PropertyType == typeof(byte[]))
            {
                // TODO: check if is encrypted field.
                // if so - build proper lamda
                throw new NotImplementedException();
            }

            #endregion

            tasks.Add(new KeyTask<object?>()
            {
                Key = key,
                Task = Task.FromResult<object?>(null)
            });
        }


        return tasks.ToDictionary(task => task.Key, task => task switch
        {
            KeyTask<long> t => t.Task,
            KeyTask<int> t => t.Task,
            KeyTask<double> t => t.Task,
            KeyTask<decimal> t => t.Task,
            KeyTask<DateTime> t => t.Task,
            KeyTask<string> t => t.Task,
            KeyTask<object> t => t.Task,
            _ => null
        });
    }

    public static async Task<object?> AggregateAsync<TModel, TDto>(this IQueryable<TModel> dbSet,
        AggregateType aggregateType, string columnName, CancellationToken cancellationToken = default)
        where TModel : class
    {
        var aggregates = new List<AggregateDto>
        {
            new()
            {
                AggregateType = aggregateType,
                PropertyName = columnName
            }
        };

        var result = await dbSet.GetAggregatesAsync<TModel, TDto>(aggregates, cancellationToken);

        return result.First();
    }


    private abstract class ImTask
    {
        public string Key = null!;
    }

    private class KeyTask<T> : ImTask
    {
        public T Task = default!;
    }
}

internal class EncryptedConverter : IConverter<string, byte[]>
{
    public byte[] ConvertTo(string from)
    {
        return Pandatech.Crypto.Aes256.EncryptWithHash(from);
    }

    public string ConvertFrom(byte[] to)
    {
        return Pandatech.Crypto.Aes256.DecryptIgnoringHash(to);
    }
}