using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.EntityFilters;

public class OrderFilter
{
    [MappedToProperty(nameof(Order.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    public string Id { get; set; }

    [MappedToProperty(nameof(Order.Quantity))]
    public long Quantity { get; set; }
    
    [MappedToProperty(nameof(Order.VerifiedQuantity))]
    public long? VerifiedQuantity { get; set; }

    [MappedToProperty(nameof(Order.TotalAmount))]
    public decimal TotalAmount { get; set; }
    
    [MappedToProperty(nameof(Order.MinSize))]
    public decimal MinSize { get; set; }
    
    [MappedToProperty(nameof(Order.Discount))]
    public decimal? Discount { get; set; }

    [MappedToProperty(nameof(Order.PaymentStatus))]
    public PaymentStatus PaymentStatus { get; set; }
    
    [MappedToProperty(nameof(Order.CancellationStatus))]
    public CancellationStatus CancellationStatus { get; set; }
    
    [MappedToProperty(nameof(Order.Paid))]
    public bool Paid { get; set; }
    
    [MappedToProperty(nameof(Order.Returned))]
    public bool? Returned { get; set; }
    
    [MappedToProperty(nameof(Order.CreatedAt))]

    public DateTime CreatedAt { get; set; }
    
    [MappedToProperty(nameof(Order.CustomerId), ConverterType = typeof(FilterPandaBaseConverter))]
    public int CustomerId { get; set; }
    
    public CustomerFilter Customer { get; set; } = null!;
}