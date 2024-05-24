using System.Linq.Expressions;
using System.Text.Json;
using BaseConverter;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Helpers;

internal static class PropertyHelper
{
    public static List<T> GetValues<T>(this FilterDto filter, MappedToPropertyAttribute propertyAttribute,
        DbContext? context = null)
    {
        var converterType = propertyAttribute.Encrypted
            ? typeof(EncryptedConverter)
            : propertyAttribute.ConverterType ?? typeof(DirectConverter);

        propertyAttribute.ConverterType = converterType;

        var sourceType = converterType == typeof(DirectConverter)
            ? typeof(T)
            : converterType.GetMethod("ConvertFrom")!.ReturnType;
        var converter = Activator.CreateInstance(converterType) as IConverter;

        converter!.Context = context;

        var method = converter.GetType().GetMethods().First(x => x.Name == "ConvertTo");

        var list = new List<T>();

        foreach (var value in filter.Values)
        {
            if (value is null)
            {
                if (sourceType != typeof(string) && Nullable.GetUnderlyingType(sourceType) == null &&
                    !propertyAttribute.Encrypted)
                {
                    throw new UnsupportedValueException($"Property {filter.PropertyName} cannot have null value");
                }

                list.Add((T)value);
                continue;
            }

            var fromJsonElementType =
                Nullable.GetUnderlyingType(sourceType) != null ? Nullable.GetUnderlyingType(sourceType)! : sourceType;

            var fromJsonElementMethod =
                typeof(PropertyHelper).GetMethod("FromJsonElement")!.MakeGenericMethod(fromJsonElementType);
            var val = fromJsonElementMethod.Invoke(null, [value, propertyAttribute])!;

            // Check supported characters for Base Converter
            if (converter is FilterPandaBaseConverter)
            {
                var base36Chars = PandaBaseConverter.Base36Chars;
                if (val is not null && val.ToString()!.Any(c => !base36Chars.Contains(c.ToString())))
                {
                    filter.Values = [];
                    continue;
                }
            }

            var valConverted = method.Invoke(converter, [val])!;

            list.Add((T)valConverted);
        }

        return list;
    }

    public static T? FromJsonElement<T>(object? value, MappedToPropertyAttribute attribute)
    {
        if (value is not JsonElement val)
        {
            return (T)Convert.ChangeType(value, typeof(T))!;
        }

        if (typeof(T).EnumCheck())
        {
            if (val.ValueKind == JsonValueKind.String)
                return (T)Enum.Parse(typeof(T).GetEnumType(), val.GetString()!, true);

            return (T)Enum.ToObject(typeof(T), val.GetInt32());
        }

        var type = attribute.Encrypted ? typeof(string) : typeof(T);

        if (val.ValueKind == JsonValueKind.Null)
            return default;

        if (val.ValueKind == JsonValueKind.Undefined)
            return default;

        if (type == typeof(string))
            return (T)(object)(attribute.Encrypted ? val.GetString()! : val.GetString()!.ToLower());

        if (type == typeof(byte) || type == typeof(byte?))
            return (T)(object)val.GetByte();

        if (type == typeof(sbyte) || type == typeof(sbyte?))
            return (T)(object)val.GetSByte();

        if (type == typeof(short) || type == typeof(ushort) ||
            type == typeof(short?) || type == typeof(ushort?))
            return (T)(object)val.GetInt16();

        if (type == typeof(int) || type == typeof(int?) || type == typeof(uint) || type == typeof(uint?))
            return (T)(object)val.GetInt32();

        if (type == typeof(long) || type == typeof(ulong) || type == typeof(long?) || type == typeof(ulong?))
            return (T)(object)val.GetInt64();

        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float) || type == typeof(decimal?) ||
            type == typeof(double?) || type == typeof(float?))
            return (T)(object)val.GetDecimal();

        if (type == typeof(bool) || type == typeof(bool?))
            return val.GetBoolean() ? (T)(object)true : (T)(object)false;

        if (type == typeof(DateTime) || type == typeof(DateTime?))
            return (T)(object)val.GetDateTime();

        if (type == typeof(TimeSpan) || type == typeof(TimeSpan?))
            return (T)(object)TimeSpan.Parse(val.ToString());

        if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            return (T)(object)val.GetDateTimeOffset();

        if (type == typeof(Guid) || type == typeof(Guid?))
            return (T)(object)val.GetGuid();

        return Activator.CreateInstance<T>()!;
    }

    public static string GetPropertyLambda(MappedToPropertyAttribute propertyAttribute)
    {
        var properties = new List<string>();

        properties.Add(propertyAttribute.TargetPropertyName);
        properties.AddRange(propertyAttribute.SubProperties);

        return string.Join(".", properties);
    }

    public static Type GetPropertyType(Type modelType, MappedToPropertyAttribute propertyAttribute)
    {
        if (propertyAttribute.Encrypted) return typeof(byte[]);
        
        var propertyType = modelType.GetProperty(propertyAttribute.TargetPropertyName)?.PropertyType;

        if (propertyType is null)
            throw new PropertyNotFoundException(
                $"Property {propertyAttribute.TargetPropertyName} not found in {modelType.Name}");

        foreach (var subProperty in propertyAttribute.SubProperties)
        {
            var subPropertyType = propertyType!.GetProperty(subProperty)?.PropertyType;
            if (propertyType is null)
                throw new PropertyNotFoundException(
                    $"Property {subProperty} not found in {propertyType!.Name}");

            propertyType = subPropertyType;
        }

        return propertyType!;
    }

    public static MemberExpression GetPropertyExpression(ParameterExpression parameter,
        MappedToPropertyAttribute propertyAttribute)
    {
        var properties = GetPropertyLambda(propertyAttribute).Split('.');
        var property = properties.Aggregate<string, Expression>(parameter, Expression.Property);
        return (MemberExpression)property;
    }
}