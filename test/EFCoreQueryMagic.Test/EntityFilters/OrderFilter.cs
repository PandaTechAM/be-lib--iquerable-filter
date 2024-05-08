using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;
using Microsoft.EntityFrameworkCore;

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
    public CancellationStatus? CancellationStatus { get; set; }
    
    [MappedToProperty(nameof(Order.CancellationStatus), ConverterType = typeof(NullableEnumStringConverter))]
    public CancellationStatus? CancellationStatus2 { get; set; }
    
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

public class NullableEnumStringConverter : IConverter<string?, CancellationStatus?>
{
    public DbContext? Context { get; set; }
    public string? ConvertFrom(CancellationStatus? from)
    {
        var value = from is null ? (int?)null : (int)from.Value;
        return value?.ToString() ?? null;
    }

    public CancellationStatus? ConvertTo(string? to)
    {
        return to is null ? null : Enum.Parse<CancellationStatus>(to);
    }
}