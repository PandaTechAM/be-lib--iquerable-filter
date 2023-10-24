using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class MappedToPropertyAttribute : Attribute
{
    public Type? FilterType;
    
    public Type? TargetConverterType;
    public Type? BackwardConverterType;
    public readonly string TargetPropertyName;
    public ComparisonType[]? ComparisonTypes;

    public bool Sortable = true;
    public MappedToPropertyAttribute(string property)
    {
        TargetPropertyName = property;
    }
}