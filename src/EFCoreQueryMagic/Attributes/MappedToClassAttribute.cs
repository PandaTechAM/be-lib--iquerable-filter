namespace EFCoreQueryMagic.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MappedToClassAttribute(Type type) : Attribute
{
    public readonly Type? TargetType = type;
}