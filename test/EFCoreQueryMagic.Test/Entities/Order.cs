using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(OrderFilter))]
public class Order
{
    public long Id { get; set; }
    public long Quantity { get; set; }
    public decimal? Min { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }


    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}