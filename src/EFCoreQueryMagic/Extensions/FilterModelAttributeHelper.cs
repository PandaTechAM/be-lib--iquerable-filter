using System.Reflection;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Extensions;

public static class FilterModelAttributeHelper
{
    public static Type GetTargetType(this Type @class)
    {
        var filterModelAttribute = @class.GetCustomAttribute<FilterModelAttribute>() ??
                                   throw new MappingException($"Model {@class.Name} is not mapped to any filter class");
        return filterModelAttribute.TargetType;
    }
}