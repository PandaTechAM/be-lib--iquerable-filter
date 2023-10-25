using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Helpers;

namespace PandaTech.IEnumerableFilters.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class MappedToPropertyAttribute : Attribute
{
    public Type? TargetConverterType;
    public Type? BackwardConverterType;
    public readonly string TargetPropertyName;
    public ComparisonType[]? ComparisonTypes;

    public readonly bool Sortable = true;

    public MappedToPropertyAttribute(string property)
    {
        TargetPropertyName = property;
    }

    public ComparisonTypesDefault ComparisonTypesDefault
    {
        get => throw new Exception("This property is only for serialization");
        set
        {
            ComparisonTypes = value switch
            {
                ComparisonTypesDefault.Numeric => DefaultComparisonTypes.Numeric,
                ComparisonTypesDefault.String => DefaultComparisonTypes.String,
                ComparisonTypesDefault.DateTime => DefaultComparisonTypes.DateTime,
                ComparisonTypesDefault.Bool => DefaultComparisonTypes.Bool,
                ComparisonTypesDefault.Guid => DefaultComparisonTypes.Guid,
                ComparisonTypesDefault.Enum => DefaultComparisonTypes.Enum,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }
}