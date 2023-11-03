using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Helpers;

namespace PandaTech.IEnumerableFilters.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class MappedToPropertyAttribute : Attribute
{
    public Type? ConverterType;
    public readonly string TargetPropertyName;
    public ComparisonType[]? ComparisonTypes;
    public string SubPropertyRoute = "";

    public bool Encrypted = false;
    public bool Sortable = true;

    public MappedToPropertyAttribute(string property)
    {
        TargetPropertyName = property;
    }
}