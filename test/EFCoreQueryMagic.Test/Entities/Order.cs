using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

public class Order
{
    public int Id { get; set; }
    
    public int Quantity { get; set; }

    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public DateTime CreatedAt { get; set; }


    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}