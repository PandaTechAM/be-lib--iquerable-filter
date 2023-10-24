using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MappedToClassAttribute : Attribute
{
    public readonly Type? TargetType;
    
    public MappedToClassAttribute(Type type)
    {
        TargetType = type;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class MappedToPropertyAttribute : Attribute
{
    public Type? FilterType;
    
    public Type? TargetConverterType;
    public Type? BackwardConverterType;
    public readonly string TargetPropertyName;
    public ComparisonType[]? ComparisonTypes;

    public MappedToPropertyAttribute(string property)
    {
        TargetPropertyName = property;
    }
}


