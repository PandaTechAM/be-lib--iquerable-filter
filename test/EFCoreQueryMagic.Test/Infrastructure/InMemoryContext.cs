using EFCoreQueryMagic.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.Infrastructure
{
    public class InMemoryContext(DbContextOptions<InMemoryContext> options)
        : DbContext(options) // Inherit from DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        
        public static InMemoryContext CreateNewInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<InMemoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for the database to avoid conflicts between tests
                .Options;

            return new InMemoryContext(options);
        }
    }
}