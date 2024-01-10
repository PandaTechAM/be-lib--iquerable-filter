using System.Reflection;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Extensions;

public static class FilterModelAttributeHelper
{
    public static Type GetTargetType(this Type @class)
    {
        var filterModelAttribute = @class.GetCustomAttribute<FilterModelAttribute>() ??
                                   throw new MappingException($"Model {@class.Name} is not mapped to any filter class");
        return filterModelAttribute.TargetType;
    }
}