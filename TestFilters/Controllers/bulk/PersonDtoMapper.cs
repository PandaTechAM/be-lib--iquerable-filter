using PandaTech.Mapper;
using TestFilters.Controllers.Models;

namespace TestFilters.Controllers.bulk;

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
            Id = from.PersonId,
            FavoriteCat = from.FavoriteCat,
            Sex = from.Sex,
            Cats = from.Cats?.Select(x => new CatDto { Id = x.Id, Name = x.Name, Age = x.Age}).ToList(),
            NewBirthDate = from.NewBirthDate,
            Ints = from.Ints
        };
    }

    public List<PersonDto> Map(List<Person> from)
    {
        return from.Select(Map).ToList();
    }
}