using System.Text.Json.Serialization;
using BaseConverter;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Converters;

namespace TestFilters.Controllers.Models;

[MappedToClass(typeof(Person))]
public class PersonDto
{
    public List<CatDto>? Cats { get; set; } = null!;

    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    [MappedToProperty(nameof(Person.PersonId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long Id { get; set; }

    public long RealId => Id;

    public List<int> Ints { get; set; } = null!;

    [MappedToProperty(nameof(Person.BirthDate))]
    public DateTime? NewBirthDate { get; set; }
    public Dummy? FavoriteCat { get; set; } = null!;
    public Sex Sex { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }

    public DateOnly BirthDate => DateOnly.FromDateTime(DateTime.Now).AddYears(-Age);
    public DateTime Now => DateTime.UtcNow;

    public List<MyEnum> Enums { get; set; } = null!;
}