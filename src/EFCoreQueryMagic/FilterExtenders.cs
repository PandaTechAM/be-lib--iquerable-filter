using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Helpers;

namespace EFCoreQueryMagic;

public static class FilterExtenders
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

    public static Task<List<FilterInfo>> GetFiltersAsync(Assembly assembly, string tableName)
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
                                   ??
                                   [
                                       ComparisonType.Equal,
                                       ComparisonType.NotEqual,
                                       ComparisonType.In,
                                       ComparisonType.NotIn
                                   ]).ToList(),
                isEncrypted = x.GetCustomAttribute<MappedToPropertyAttribute>()!.Encrypted
            }
        );

        return Task.FromResult(infos.ToList());
    }

    public static Task<List<FilterInfo>> GetFiltersAsync(string tableName) =>
        GetFiltersAsync(Assembly.GetCallingAssembly(), tableName);
}