using System.Text.Json.Serialization;
using BaseConverter;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.Mapper;

namespace TestFilters.Controllers.Models;

[MappedToClass(typeof(Cat))]
public class CatDto
{
    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    [MappedToProperty(nameof(Cat.Id), FilterType = typeof(string), TargetConverterType = typeof(PandaFilterBaseConverter))]
    public long Id { get; set; }
    [MappedToProperty(nameof(Cat.Name))]
    public string Name { get; set; } = null!;
    [MappedToProperty(nameof(Cat.Age))]
    public int Age { get; set; }
}

public class PandaFilterBaseConverter : IConverter<string, long>
{
    public long Convert(string from)
    {
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }
}