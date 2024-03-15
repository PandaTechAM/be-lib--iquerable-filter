using EFCoreQueryMagic.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.Infrastructure
{
    public class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options) // Inherit from DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        
        public static TestDbContext CreateNewInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for the database to avoid conflicts between tests
                .Options;

            return new TestDbContext(options);
        }
    }
}