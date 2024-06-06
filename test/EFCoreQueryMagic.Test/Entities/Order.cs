using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(OrderFilter))]
public class Order : IEquatable<Order>
{
    public long Id { get; set; }
    public long Quantity { get; set; }
    public long? VerifiedQuantity { get; set; }
    public decimal MinSize { get; set; }
    public decimal? Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public CancellationStatus? CancellationStatus { get; set; }
    public bool Paid { get; set; }
    public bool? Returned { get; set; }
    public DateTime CreatedAt { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public List<Item> Items { get; set; } = null!;

    public bool Equals(Order? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Order)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(Quantity);
        hashCode.Add(VerifiedQuantity);
        hashCode.Add(MinSize);
        hashCode.Add(Discount);
        hashCode.Add(TotalAmount);
        hashCode.Add((int)PaymentStatus);
        hashCode.Add(CancellationStatus);
        hashCode.Add(Paid);
        hashCode.Add(Returned);
        hashCode.Add(CreatedAt);
        hashCode.Add(CustomerId);
        hashCode.Add(Customer);
        hashCode.Add(Items);
        return hashCode.ToHashCode();
    }
}