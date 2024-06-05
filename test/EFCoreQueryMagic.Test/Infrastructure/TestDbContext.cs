using EFCoreQueryMagic.PostgresContext;
using EFCoreQueryMagic.Test.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.Infrastructure
{
    public class TestDbContext(DbContextOptions<TestDbContext> options)
        : PostgresDbContext(options) // Inherit from DbContext
    {
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<ItemType> ItemTypes { get; set; } = null!;
        public DbSet<ItemTypeMapping> ItemTypeMappings { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CategoryName> CategoryNames { get; set; } = null!;
        
        public static TestDbContext CreateNewInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseNpgsql("Host=localhost;Database=filter_tests;Username=test;Password=test")
               // .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use a unique name for the database to avoid conflicts between tests
                .Options;

            var context = new TestDbContext(options);

            
            
            return context;
        }
    }
}