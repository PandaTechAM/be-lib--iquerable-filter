using System.ComponentModel.DataAnnotations.Schema;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;
using PandaTech.IEnumerableFilters.PostgresContext;

namespace TestFilters.db;

public class PostgresContext(DbContextOptions<PostgresContext> options, Aes256 aes256) : PostgresDbContext(options)
{
    public virtual DbSet<Company> Companies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .OwnsOne(c => c.Info, d => { d.ToJson(); });

        base.OnModelCreating(modelBuilder);
    }

    public async Task Populate(int count)
    {
        await Database.EnsureDeletedAsync();
        await Database.EnsureCreatedAsync();

        var fake = new Faker<Company>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.NameEncrypted, (f, e) => aes256.Encrypt(e.Name))
            .RuleFor(x => x.Age, f => f.Random.Long(1, 100))
            .RuleFor(x => x.IsEnabled, f => f.Random.Bool())
            .RuleFor(x => x.Info, f => new Info { Name = f.Company.CompanyName() })
            .RuleFor(x => x.NullableString, f => f.Random.Bool() ? f.Company.CompanyName() : null)
            .RuleFor(x => x.Types, f => new[]
            {
                f.PickRandom<CType>(),
                f.PickRandom<CType>(),
                f.PickRandom<CType>(),
                f.PickRandom<CType>(),
                f.PickRandom<CType>()
            }.Distinct().ToArray())
            .RuleFor(x => x.Type, f => f.PickRandom<CType>());

        var generatedData = fake.Generate(count);

        await AddRangeAsync(generatedData);
        await SaveChangesAsync();
    }
}