using PandaTech.IEnumerableFilters.Enums;

namespace PandaTech.IEnumerableFilters.Attributes;


[AttributeUsage(AttributeTargets.Property)]
public class OrderAttribute : Attribute
{
    public readonly int Order;
    public readonly OrderDirection Direction;

    public OrderAttribute(int order = 1, OrderDirection direction = OrderDirection.Ascending)
    {
        Order = order;
        Direction = direction;
    }
}