using System.Text.Json.Serialization;
using BaseConverter;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.Mapper;

namespace TestFilters.Controllers;

[PrimaryKey(nameof(Id))]
public class Person
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    
    public double Money { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsMarried { get; set; }
    public bool IsWorking { get; set; }
    public bool IsHappy { get; set; }
    
    public List<Cat> Cats { get; set; } = null!;
}

public class PersonDto
{
    public string Cats { get; set; } = null!;
    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    public long Id { get; set; }
    public long RealId => Id;
    public string FullName { get; set; } = null!;
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
            FullName = $"{from.Name} {from.Surname}",
            Age = from.Age,
            Email = from.Email,
            Id = from.Id,
            Cats = string.Join(", ", from.Cats.Select(c => c.Name))
        };
    }

    public List<PersonDto> Map(List<Person> from)
    {
        return from.Select(Map).ToList();
    }
}