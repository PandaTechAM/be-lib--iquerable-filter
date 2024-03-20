using System.Text;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;
using Pandatech.Crypto;
using Random = System.Random;

namespace EFCoreQueryMagic.Test.Infrastructure;

public class DatabaseFixture : IDisposable
{
    public TestDbContext Context { get; private set; }
    public Aes256 Aes256 { get; private set; }

    public DatabaseFixture()
    {
        Context = TestDbContext.CreateNewInMemoryContext();

        Aes256 = new Aes256(new Aes256Options
        {
            Key = "M5pfvJCKBwpJdA7YfeX3AkAKJmfBf4piybEPDtWKWw4="
        });
        
        SeedData(Context);
    }

    private void SeedData(TestDbContext context)
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Categories = [CategoryType.Average, CategoryType.Good] },
            new() { Id = 2, Categories = [CategoryType.Poor, CategoryType.Average] }
        };

        context.Categories.AddRange(categories);

        var customers = new List<Customer>
        {
            new()
            {
                Id = 1, Name = GenerateRandomBytes(10), Email = "customer1@example.com", Age = 18,
                TotalOrders = 1, BirthDay = DateTime.UtcNow, SocialId = ConvertToByteArray("1234567890"),
                CreatedAt = DateTime.UtcNow, CategoryId = 1
            },
            new()
            {
                Id = 2, Name = GenerateRandomBytes(10), Email = "customer2@example.com", Age = null,
                TotalOrders = 10, BirthDay = null, SocialId = null, CreatedAt = DateTime.UtcNow, CategoryId = 2
            }
        };

        context.Customers.AddRange(customers);

        var orders = new List<Order>
        {
            new()
            {
                Id = 1, TotalAmount = 100.50m, Min = 1L, PaymentStatus = PaymentStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-1), CustomerId = 1
            },
            new()
            {
                Id = 2, TotalAmount = 200.00m, Min = null, PaymentStatus = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow, CustomerId = 2
            }
        };

        context.Orders.AddRange(orders);

        context.SaveChanges();
    }

    private static byte[] GenerateRandomBytes(int length)
    {
        var randomBytes = new byte[length];
        new Random().NextBytes(randomBytes);
        return randomBytes;
    }

    private byte[] ConvertToByteArray(string value)
    {
        return Aes256.Encrypt(value, false);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}