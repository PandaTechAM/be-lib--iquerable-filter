using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Infrastructure;

public class DatabaseFixture : IDisposable
{
    public TestDbContext Context { get; private set; }

    public DatabaseFixture()
    {
        Context = TestDbContext.CreateNewInMemoryContext();
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
            new() { Id = 1, Name = GenerateRandomBytes(10), Email = "customer1@example.com", CategoryId = 1 },
            new() { Id = 2, Name = GenerateRandomBytes(10), Email = "customer2@example.com", CategoryId = 2 }
        };

        context.Customers.AddRange(customers);

        var orders = new List<Order>
        {
            new()
            {
                Id = 1, TotalAmount = 100.50m, PaymentStatus = PaymentStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-1), CustomerId = 1
            },
            new()
            {
                Id = 2, TotalAmount = 200.00m, PaymentStatus = PaymentStatus.Pending, CreatedAt = DateTime.UtcNow,
                CustomerId = 2
            }
        };

        context.Orders.AddRange(orders);

        context.SaveChanges();
    }

    private byte[] GenerateRandomBytes(int length)
    {
        var randomBytes = new byte[length];
        new Random().NextBytes(randomBytes);
        return randomBytes;
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}