using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(CustomerFilter))]
public class Customer
{
    public long Id { get; set; }
    public byte[] Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? Age { get; set; }
    public int TotalOrders { get; set; }
    public byte[]? SocialId { get; set; }
    public DateTime? BirthDay { get; set; }
    public DateTime CreatedAt { get; set; }

    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}