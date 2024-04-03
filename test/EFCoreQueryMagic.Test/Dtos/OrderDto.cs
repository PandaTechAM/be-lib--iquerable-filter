using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Dtos;

public class OrderDto
{
    public string Id { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CustomerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string CategoryId { get; set; } = null!;
    public List<CategoryType> Categories { get; set; } = null!;
}