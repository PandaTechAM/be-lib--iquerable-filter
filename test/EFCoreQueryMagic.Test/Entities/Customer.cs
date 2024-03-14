namespace EFCoreQueryMagic.Test.Entities;

public class Customer
{
    public int Id { get; set; }
    public byte[] Name { get; set; } = null!;
    public string Email { get; set; } = null!;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}