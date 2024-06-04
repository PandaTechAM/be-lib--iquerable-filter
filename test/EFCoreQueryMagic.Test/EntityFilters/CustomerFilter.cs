using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.EntityFilters;

public class CustomerFilter
{
    [MappedToProperty(nameof(Customer.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    [Order]
    public long Id { get; set; }

    [MappedToProperty(nameof(Customer.FirstName), Encrypted = true)]
    public byte[] FirstName { get; set; } = null!;
    
    [MappedToProperty(nameof(Customer.LastName))]
    public byte[] LastName { get; set; } = null!;

    [MappedToProperty(nameof(Customer.SomeByteArray))]
    public byte[]? SomeByteArray { get; set; }
    
    [MappedToProperty(nameof(Customer.Email))]
    public string Email { get; set; } = null!;
    
    [MappedToProperty(nameof(Customer.PhoneNumber))]
    public string? PhoneNumber { get; set; }
    
    [MappedToProperty(nameof(Customer.Age))]
    public int? Age { get; set; }

    [MappedToProperty(nameof(Customer.TotalOrders))]
    public int TotalOrders { get; set; }
    
    [MappedToProperty(nameof(Customer.SocialId), Encrypted = true)]
    public byte[]? SpecialDocumentId { get; set; }
    
    [MappedToProperty(nameof(Customer.BirthDay))]
    public DateTime? BirthDay { get; set; }
    
    [MappedToProperty(nameof(Customer.CreatedAt))]
    public DateTime CreatedAt { get; set; }
    
    [MappedToProperty(nameof(Customer.OrderId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long? OrderId { get; set; }
    
    public OrderFilter Order { get; set; } = null!;

    [MappedToProperty(nameof(Customer.CategoryId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long CategoryId { get; set; }

    [MappedToProperty(nameof(Customer.Statuses))]
    public CustomerStatus[]? Statuses { get; set; }

    [MappedToProperty(nameof(Customer.Types))]
    public CustomerType[] Types { get; set; } = null!;
    
    public CategoryFilter Category { get; set; } = null!;
}