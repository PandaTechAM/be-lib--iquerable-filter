using System.Linq.Expressions;
using System.Text.Json;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters;

internal static class PropertyHelper
{
    public static List<T> GetValues<T>(this FilterDto filter, MappedToPropertyAttribute propertyAttribute)
    {
        var converter = Activator.CreateInstance(propertyAttribute.ConverterType ?? typeof(DirectConverter));
        var sourceType = propertyAttribute.ConverterType is not null
            ? propertyAttribute.ConverterType.GetMethod("ConvertFrom")!.ReturnType
            : typeof(T);
        var method = converter!.GetType().GetMethods().First(x => x.Name == "ConvertTo");
        
        var list = new List<T>();
        foreach (var value in filter.Values)
        {
            var fromJsonElementMethod = typeof(PropertyHelper).GetMethod("FromJsonElement")!.MakeGenericMethod(sourceType);    
            var val = fromJsonElementMethod.Invoke(null, [value, propertyAttribute])!;
            
            if (typeof(T).IsEnum && val is string s)
                val = Enum.Parse(typeof(T), s, true);
            
            var val2 = method.Invoke(converter, [val])!;
            
            list.Add((T) val2);
        }
        
        return list;
    }

    public static T FromJsonElement<T>(JsonElement val, MappedToPropertyAttribute attribute)
    {
        return (T)(typeof(T).Name switch
        {
            "String" => attribute.Encrypted ? val.GetString()! : val.GetString()!.ToLower(),
            "Int32" => val.GetInt32(),
            "Int64" => val.GetInt64(),
            "Boolean" => val.GetBoolean(),
            "DateTime" => val.GetDateTime(),
            "Decimal" => val.GetDecimal(),
            "Double" => val.GetDouble(),
            "Single" => val.GetSingle(),
            "Guid" => val.GetGuid(),
            _ => Activator.CreateInstance(typeof(T))!
        });

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