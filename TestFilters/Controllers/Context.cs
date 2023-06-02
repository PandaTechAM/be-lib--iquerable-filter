using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace TestFilters.Controllers;

public class Context : DbContext
{
    public virtual DbSet<Person> Persons { get; set; } = null!;
    public virtual DbSet<Cat> Cats { get; set; } = null!;
    public virtual DbSet<Phrase> Phrases { get; set; }


    public Context(DbContextOptions<Context> options) : base(options)
    {
    }
}