using System.Text.Json.Serialization;
using BaseConverter;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Helpers;

namespace TestFilters.Controllers.Models;

[MappedToClass(typeof(Cat))]
public class CatDto
{
    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    [MappedToProperty(nameof(Cat.Id),
        ConverterType = typeof(PandaFilterBaseConverter))]
    public long Id { get; set; }

    [MappedToProperty(nameof(Cat.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Cat.Age))]
    public int Age { get; set; }
    
    [MappedToProperty(nameof(Cat.SomeBytes), Encrypted = true)]
    public string EncryptedString { get; set; } = null!;
    
    [MappedToProperty(nameof(Cat.Types), SubPropertyRoute = nameof(CatTypes.Name))]
    public string CatType { get; set; } = null!;
}

public class PandaFilterBaseConverter : IConverter<string, long>
{
    public long Convert(string from)
    {
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }

    public long ConvertTo(string from)
    {
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }

    public string ConvertFrom(long to)
    {
        return PandaBaseConverter.Base10ToBase36(to)!;
    }
}

