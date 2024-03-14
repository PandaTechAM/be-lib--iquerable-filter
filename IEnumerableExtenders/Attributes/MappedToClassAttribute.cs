namespace PandaTech.IEnumerableFilters.Attributes;

[AttributeUsage(AttributeTargets.Class)]
[Obsolete]
public class MappedToClassAttribute : Attribute
{
    public readonly Type? TargetType;

    public MappedToClassAttribute(Type type)
    {
        TargetType = type;
    }
}