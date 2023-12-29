using System.Reflection;
using PandaTech.IEnumerableFilters.Exceptions;

namespace PandaTech.IEnumerableFilters.Attributes;

public static class FilterModelAttributeHelper
{
    public static Type GetTargetType(this Type attribute)
    {
        var filterModelAttribute = attribute.GetCustomAttribute<FilterModelAttribute>() ??
                                   throw new MappingException($"Model {attribute.Name} is not mapped to any filter class");
        return filterModelAttribute.TargetType;
    }
}