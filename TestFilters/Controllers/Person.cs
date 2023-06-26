using System.Text.Json.Serialization;
using BaseConverter;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.Mapper;

namespace TestFilters.Controllers;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Sex
{
    male,
    female
}

public class Dummy 
{
    public Guid Id { get; set; }

    public static bool operator ==(Dummy l, Dummy r) => r.Id == l.Id;

    public static bool operator !=(Dummy l, Dummy r) => !(l.Id == r.Id);
}

[PrimaryKey(nameof(Id))]
public class Person
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Sex Sex { get; set; }
    public int Age { get; set; }

    public Dummy? FavoriteCat { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public double Money { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsMarried { get; set; }
    public bool IsWorking { get; set; }
    public bool IsHappy { get; set; }
    public DateTime? NewBirthDate { get; set; }

    public List<Cat>? Cats { get; set; } = null!;
}

public class PersonDto
{
    public List<Cat>? Cats { get; set; } = null!;

    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    public long Id { get; set; }

    public long RealId => Id;

    public DateTime? NewBirthDate { get; set; }
    public Dummy? FavoriteCat { get; set; } = null!;
    public Sex Sex { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }

    public DateOnly BirthDate => DateOnly.FromDateTime(DateTime.Now).AddYears(-Age);
    public DateTime Now => DateTime.UtcNow;
}

public class PersonDtoMapper : IMapping<Person, PersonDto>
{
    public PersonDto Map(Person from)
    {
        return new PersonDto
        {
            Name = from.Name,
            Surname = from.Surname,
            Age = from.Age,
            Email = from.Email,
            Id = from.Id,
            FavoriteCat = from.FavoriteCat,
            Sex = from.Sex,
            Cats = from.Cats,
            NewBirthDate = from.NewBirthDate
        };
    }

    public List<PersonDto> Map(List<Person> from)
    {
        return from.Select(Map).ToList();
    }
}