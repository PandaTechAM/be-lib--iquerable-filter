namespace EFCoreQueryMagic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class FilterModelAttribute(Type targetType) : Attribute //todo: Do we need both this attribute and MappedToClassAttribute?
{
    public readonly Type TargetType = targetType;
}