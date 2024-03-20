using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Dtos;

public class CustomerDto
{
    public long Id { get; set; }
    public byte[] Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int? Age { get; set; }
    public int TotalOrders { get; set; }
    public byte[]? SocialId { get; set; }
    public DateTime? BirthDay { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string CategoryId { get; set; } = null!;
    public List<CategoryType> Categories { get; set; } = null!;
}