using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Helpers;
using EFCoreQueryMagic.PostgresContext;
using static System.Linq.Expressions.Expression;

namespace EFCoreQueryMagic;

public static partial class FilterExtenders
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
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = type.GetGenericArguments()[0];
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

    private static bool ValidateFilter<TModel, TDto>(FilterDto filterDto)
    {
        var dtoType = typeof(TDto);
        var mappedToClassAttribute = dtoType.GetCustomAttribute<MappedToClassAttribute>();
        if (mappedToClassAttribute is null)
            throw new MappingException($"Dto {dtoType.Name} is not mapped to any class");

        if (mappedToClassAttribute.TargetType != typeof(TModel))
            throw new MappingException($"Dto {dtoType.Name} is not mapped to {typeof(TModel).Name}");

        var mappedProperties = dtoType.GetProperties();

        return false;
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
            x => new FilterInfo
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
                                   }).ToList(),
                isEncrypted = x.GetCustomAttribute<MappedToPropertyAttribute>()!.Encrypted
            }
        );

        return infos.ToList();
    }

    public static List<FilterInfo> GetFilters(string tableName) => GetFilters(Assembly.GetCallingAssembly(), tableName);
}

public static partial class FilterExtenders
{
    public static IQueryable<TModel> ApplyFilters<TModel, TDto>(this IQueryable<TModel> dbSet, List<FilterDto> filters)
    {
        return dbSet.ApplyFilters(filters);

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

            if (filterDto.Values.Count == 0)
                switch (filterDto.ComparisonType)
                {
                    case ComparisonType.IsTrue:
                        break;
                    case ComparisonType.IsFalse:
                        break;
                    default:
                        return dbSet.Where(x => false);
                }

            var targetProperty = typeof(TModel).GetProperty(filter.Attribute.TargetPropertyName);
            if (targetProperty is null)
                throw new PropertyNotFoundException(
                    $"Property {filter.Attribute.TargetPropertyName} not found in {typeof(TModel).Name}");

            var sourceType = filter.Attribute.ConverterType is not null
                ? filter.Attribute.ConverterType.GetMethod("ConvertFrom")!.ReturnType
                : filter.Type;

            if (sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(List<>))
                sourceType = sourceType.GetGenericArguments()[0];

            var targetType = filter.Attribute.ConverterType is not null
                ? filter.Attribute.ConverterType.GetMethod("ConvertTo")!.ReturnType
                : filter.Type;

            var filterTypeName = sourceType.Name;

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                var val = (JsonElement)filterDto.Values[index];

                if (sourceType.IsEnum)
                {
                    var enumType = sourceType;
                    var getExpression = Call(typeof(Enum), "Parse", null,
                        Constant(enumType), Constant(val.GetString()!));

                    var lambda = Lambda<Func<object>>(getExpression).Compile();

                    filterDto.Values[index] = lambda();
                }
                else if (sourceType.Name == "List`1")
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
                        _ => Activator.CreateInstance(sourceType)!
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

                continue;
            }

            var converter =
                Activator.CreateInstance(filter.Attribute.ConverterType ?? typeof(DirectConverter));

            Type targetPropertyType = PropertyHelper.GetPropertyType(typeof(TModel), filter.Attribute);

            var finalLambda = FilterLambdaBuilder.BuildLambdaString(new FilterKey
            {
                ComparisonType = filterDto.ComparisonType,
                TargetPropertyType = targetPropertyType,
                TargetPropertyName = PropertyHelper.GetPropertyLambda(filter.Attribute),
            });

            var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertTo");

            for (var index = 0; index < filterDto.Values.Count; index++)
            {
                filterDto.Values[index] = method.Invoke(converter, new[] { filterDto.Values[index] }) ??
                                          throw new MappingException("Converter returned null");
            }

            targetType = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>)
                ? targetType.GetGenericArguments()[0]
                : targetType;

            var typedList = Activator.CreateInstance(typeof(List<>).MakeGenericType(targetType));

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
}