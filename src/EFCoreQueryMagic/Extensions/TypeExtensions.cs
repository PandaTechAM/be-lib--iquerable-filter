using System.Collections;
using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Extensions;

internal static class TypeExtensions
{
    internal static bool IsIEnumerable(this Type requestType)
    {
        var isIEnumerable = typeof(IEnumerable).IsAssignableFrom(requestType);
        var notString = !typeof(string).IsAssignableFrom(requestType);
        return isIEnumerable && notString;
    }
    
    internal static bool IsIEnumerable(this Type requestType, Type elementType)
    {
        var isIEnumerable = typeof(IEnumerable).IsAssignableFrom(requestType);
        var notString = !typeof(string).IsAssignableFrom(requestType);
        var isUnderlyingTypeEqual = requestType.GetGenericArguments()[0] == elementType;
        return isIEnumerable && notString && isUnderlyingTypeEqual;
    }
    
    internal static Type GetCollectionType(this Type requestType)
    {
        if (requestType.IsArray)
            return requestType.GetElementType()!;
        
        if (requestType.IsGenericType && requestType.GetGenericTypeDefinition().IsIEnumerable())
            return requestType.GetGenericArguments()[0];
        
        /*if (requestType.IsGenericType && requestType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return requestType.GetGenericArguments()[0];*/
        
        return requestType;
    }
    
    internal static Type GetEnumType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return type.GetGenericArguments()[0];
        
        if (type.IsArray)
            return type.GetElementType()!;
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0];
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return type.GetGenericArguments()[0];
        
        return type;
    }
    
    internal static bool EnumCheck(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return type.GetGenericArguments()[0].IsEnum;
        
        if (type.IsArray)
            return type.GetElementType()!.IsEnum;
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return type.GetGenericArguments()[0].IsEnum;
        
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return type.GetGenericArguments()[0].IsEnum;
        
        return type.IsEnum;
    }
    
    internal static Type GetTargetType(this Type @class)
    {
        var filterModelAttribute = @class.GetCustomAttribute<FilterModelAttribute>() ??
                                   throw new MappingException($"Model {@class.Name} is not mapped to any filter class");
        return filterModelAttribute.TargetType;
    }
}