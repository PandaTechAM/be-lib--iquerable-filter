using System.ComponentModel.DataAnnotations.Schema;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Enums;
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

[FilterModel(typeof(CompanyFilter))]
public class Company
{
    public long Id { get; set; }
    public long Age { get; set; }
    public string Name { get; set; }
    public byte[] NameEncrypted { get; set; }
    public bool IsEnabled { get; set; }
    public CType Type { get; set; }
    public CType[] Types { get; set; }
    public Info Info { get; set; } = null!;
}

public class Info
{
    public string Name { get; set; } = null!;
}

public class CompanyFilter
{
    [MappedToProperty(nameof(Company.Id))]
    [Order(2)]
    public long Id { get; set; }

    [MappedToProperty(nameof(Company.Age))]
    [Order(direction: OrderDirection.Descending)]
    public long Age { get; set; }

    [MappedToProperty(nameof(Company.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Company.Type))]
    public string Type { get; set; } = null!;

    [MappedToProperty(nameof(Company.Types))]
    public string Types { get; set; } = null!;

    [MappedToProperty(nameof(Company.IsEnabled))]
    public bool IsEnabled { get; set; }

    [MappedToProperty(nameof(Company.NameEncrypted), Encrypted = true, Sortable = false)]
    public string NameEncrypted { get; set; } = null!;

    [MappedToProperty(nameof(Company.Info), nameof(Company.Info.Name))]
    public string InfoName { get; set; } = null!;
}

public enum CType
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Eleven,
    Twelve,
    Thirteen,
    Fourteen,
    Fifteen
}