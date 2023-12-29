using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.PostgresContext;
using TestFilters.Controllers.Models;

namespace TestFilters.Controllers;

public class Context : PostgresDbContext
{
    public virtual DbSet<Person> Persons { get; set; } = null!;
    public virtual DbSet<Cat> Cats { get; set; } = null!;
    public virtual DbSet<Phrase> Phrases { get; set; } = null!;
    public virtual DbSet<Dummy> Dummies { get; set; } = null!;
    public virtual DbSet<CatTypes> CatTypes { get; set; } = null!;


    private IServiceProvider ServiceProvider { get; }

    public Context(DbContextOptions<Context> options, IServiceProvider serviceProvider) : base(options)
    {
        ServiceProvider = serviceProvider;
    }

    public List<PersonDto> GetPersons(GetDataRequest request, int page, int pageSize)
    {
        var mapper = ServiceProvider.GetRequiredService<PandaTech.Mapper.IMapping<Person, PersonDto>>();

        return Persons
            .Include(x => x.Cats)
            .Include(x => x.FavoriteCat)
            .ApplyFilters<Person>(request.Filters)
            .ApplyOrdering<Person, PersonDto>(request.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsEnumerable()
            .Select(mapper.Map).ToList();
    }
}