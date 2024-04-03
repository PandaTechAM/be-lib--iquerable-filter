using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(OrderFilter))]
public class Order
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
}