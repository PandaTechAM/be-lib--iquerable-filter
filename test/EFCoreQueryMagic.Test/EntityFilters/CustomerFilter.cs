using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;

namespace EFCoreQueryMagic.Test.EntityFilters;

[MappedToClass(typeof(Customer))]
public class CustomerFilter
{
    [MappedToProperty(nameof(Customer.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    public long Id { get; set; }

    [MappedToProperty(nameof(Customer.Name), Encrypted = true)]
    public byte[] Name { get; set; } = null!;

    [MappedToProperty(nameof(Customer.Email))]
    public string Email { get; set; } = null!;
    
    [MappedToProperty(nameof(Customer.Age))]
    public int? Age { get; set; }

    [MappedToProperty(nameof(Customer.TotalOrders))]
    public int TotalOrders { get; set; }
    
    [MappedToProperty(nameof(Customer.SocialId), Encrypted = true)]
    public byte[]? SocialId { get; set; }
    
    [MappedToProperty(nameof(Customer.BirthDay))]
    public DateTime? BirthDay { get; set; }
    
    [MappedToProperty(nameof(Customer.CreatedAt))]
    public DateTime CreatedAt { get; set; }
    
    [MappedToProperty(nameof(Customer.CategoryId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long CategoryId { get; set; }

    public CategoryFilter Category { get; set; } = null!;
}