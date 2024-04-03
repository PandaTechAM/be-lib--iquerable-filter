using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class MappedToPropertyAttribute : Attribute
{
    public Type? ConverterType;
    public readonly string TargetPropertyName;
    public ComparisonType[]? ComparisonTypes = null;
    public string[] SubProperties = Array.Empty<string>();

    public bool Encrypted = false;
    public bool Sortable = true;

    public MappedToPropertyAttribute(string property)
    {
        TargetPropertyName = property;
    }
    
    public MappedToPropertyAttribute(string property, params string[] subProperties)
    {
        TargetPropertyName = property;
        SubProperties = subProperties;
    }
}