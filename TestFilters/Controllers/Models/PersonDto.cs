using System.Text.Json.Serialization;
using BaseConverter;

namespace TestFilters.Controllers.Models;

public class PersonDto
{
    public List<CatDto>? Cats { get; set; } = null!;

    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    public long Id { get; set; }

    public long RealId => Id;

    public List<int> Ints { get; set; } = null!;

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