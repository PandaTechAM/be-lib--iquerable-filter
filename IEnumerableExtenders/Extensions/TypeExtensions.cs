using System.Collections;

public static class TypeExtensions
{
    public static bool IsIEnumerable(this Type requestType)
    {
        var isIEnumerable = typeof(IEnumerable).IsAssignableFrom(requestType);
        var notString = !typeof(string).IsAssignableFrom(requestType);
        return isIEnumerable && notString;
    }
    
    public static Type GetCollectionType(this Type requestType)
    {
        if (requestType.IsArray)
            return requestType.GetElementType()!;
        
        if (requestType.IsGenericType && requestType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            return requestType.GetGenericArguments()[0];
        
        if (requestType.IsGenericType && requestType.GetGenericTypeDefinition() == typeof(Nullable<>))
            return requestType.GetGenericArguments()[0];
        
        
        return requestType;
    }
    
    public static Type GetEnumType(this Type type)
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
    
    public static bool EnumCheck(this Type type)
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
}