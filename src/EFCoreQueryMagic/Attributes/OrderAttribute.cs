using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute(int order = 1, OrderDirection direction = OrderDirection.Descending) : Attribute
{
    public readonly int Order = order;
    public readonly OrderDirection Direction = direction;
}