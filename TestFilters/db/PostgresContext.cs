using Bogus;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;

namespace TestFilters.db;

public class PostgresContext : DbContext
{
    public DbSet<Company> Companies { get; set; }


    public PostgresContext(DbContextOptions<PostgresContext> options) : base(options)
    {
    }

    public async Task Populate(int count)
    {
        // create data using bogus 

        var fake = new Faker<Company>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
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
    public string Name { get; set; }
    public CType Type { get; set; }
    public CType[] Types { get; set; }
}

public class CompanyFilter
{
    [MappedToProperty(nameof(Company.Id))]
    public long Id { get; set; }
    
    [MappedToProperty(nameof(Company.Name))]
    public string Name { get; set; } = null!;
    
    [MappedToProperty(nameof(Company.Type))]
    public string Type { get; set; } = null!;
    
    [MappedToProperty(nameof(Company.Types))]
    public string Types { get; set; } = null!;
    
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