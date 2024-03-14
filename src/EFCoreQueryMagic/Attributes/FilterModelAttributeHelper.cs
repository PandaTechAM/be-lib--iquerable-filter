using System.Reflection;
using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Attributes;

public static class FilterModelAttributeHelper
{
    public static Type GetTargetType(this Type modelType)
    {
        var filterModelAttribute = modelType.GetCustomAttribute<FilterModelAttribute>() ??
                                   throw new MappingException($"Model {modelType.Name} is not mapped to any filter class");
        return filterModelAttribute.TargetType;
    }
}