using System.ComponentModel.DataAnnotations.Schema;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(CustomerFilter))]
public class Customer: IEquatable<Customer>
{
    public long Id { get; set; }

    // encrypted
    public byte[] FirstName { get; set; } = null!;
    public byte[] LastName { get; set; } = null!;
    public byte[]? SomeByteArray { get; set; }
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public int? Age { get; set; }
    public int TotalOrders { get; set; }
    public decimal Average { get; set; }
    public decimal? Maximum { get; set; }

    public CustomerStatus[]? Statuses { get; set; }
    public CustomerType[] Types { get; set; } = null!;

    // encrypted
    public byte[]? SocialId { get; set; }
    public DateTime? BirthDay { get; set; }
    public DateTime CreatedAt { get; set; }

    public long? OrderId { get; set; }
    [ForeignKey(nameof(OrderId))] public Order? Order { get; set; }

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public bool Equals(Customer? other)
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
        return Equals((Customer)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}