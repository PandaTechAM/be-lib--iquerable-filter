using Bogus;
using EFCoreQueryMagic.PostgresContext;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Demo.db;

public class PostgresContext(DbContextOptions<PostgresContext> options, Aes256 aes256) : PostgresDbContext(options)
{
    public virtual DbSet<Company> Companies { get; set; } = null!;
    public virtual DbSet<SomeClass> SomeClasses { get; set; } = null!;
    public virtual DbSet<OneToMany> OneToManys { get; set; } = null!;


    public virtual DbSet<A> As { get; set; } = null!;
    public virtual DbSet<B> Bs { get; set; } = null!;
    public virtual DbSet<C> Cs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>()
            .OwnsOne(c => c.Info, d => { d.ToJson(); });

        modelBuilder.Entity<Company>()
            .HasOne(x => x.SomeClass)
            .WithOne();
        
        modelBuilder.Entity<Company>()
            .HasMany(x=>x.OneToManys)
            .WithOne(x=>x.Company);

        modelBuilder.Entity<A>().HasOne<B>(x => x.B).WithMany();
        modelBuilder.Entity<B>().HasOne<C>(x => x.C).WithMany();
        
        base.OnModelCreating(modelBuilder);
    }

    public async Task Populate(int count)
    {
        await Database.EnsureDeletedAsync();
        await Database.EnsureCreatedAsync();

        var fake = new Faker<Company>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Amount, f => f.Random.Decimal(100, 15000))
            .RuleFor(x => x.Quantity, f => f.Random.Int(1, 120))
            .RuleFor(x => x.OneToManys, f =>
                Enumerable.Range(1, 20).Select(x =>
                new OneToMany
                {
                    Name = "name",
                    Address = "Address " + f.Random.Int(1, 10000),
                }
            ).ToList())
            .RuleFor(x => x.SomeClass, f => f.Random.Bool()
                ? new SomeClass
                {
                    Name = f.Company.CompanyName(),
                    NameEncrypted = aes256.Encrypt(f.Company.CompanyName()),
                    NullableString = f.Random.Bool() ? f.Company.CompanyName() : null
                }
                : null)
            .RuleFor(x => x.NullableAge, f => f.Random.Bool() ? f.Random.Long(1, 100) : null)
            .RuleFor(x => x.NameEncrypted, (f, e) => f.Random.Bool() ? aes256.Encrypt(e.Name) : null)
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

    public async Task PopulateTest()
    {
        const int cAmount = 1000;
        const int bAmount = 1000;
        const int aAmount = 1000;

        var cFaker = new Faker<C>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName());

        Cs.AddRange(cFaker.Generate(cAmount));

        await SaveChangesAsync();

        var cs = await Cs.ToListAsync();

        var bFaker = new Faker<B>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.C, f => f.PickRandom(cs));

        Bs.AddRange(bFaker.Generate(bAmount));

        await SaveChangesAsync();

        var bs = await Bs.ToListAsync();

        var aFaker = new Faker<A>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.B, f => f.PickRandom(bs));

        As.AddRange(aFaker.Generate(aAmount));

        await SaveChangesAsync();
    }
}

public class A
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public B B { get; set; } = null!;
}

public class B
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public C C { get; set; } = null!;
}

public class C
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}