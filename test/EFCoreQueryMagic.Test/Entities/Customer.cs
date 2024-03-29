using System.ComponentModel.DataAnnotations.Schema;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(CustomerFilter))]
public class Customer
{
    public long Id { get; set; }
    // encrypted
    public byte[] FirstName { get; set; } = null!;
    public byte[] LastName { get; set; } = null!;
    public byte[]? MiddleName { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int? Age { get; set; }
    public int TotalOrders { get; set; }
    public decimal Average { get; set; }
    public decimal? Maximum { get; set; }
    // encrypted
    public byte[]? SocialId { get; set; }
    public DateTime? BirthDay { get; set; }
    public DateTime CreatedAt { get; set; }

    public long? OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }
    
    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}