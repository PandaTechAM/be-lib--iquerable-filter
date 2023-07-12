using System.Text.Json.Serialization;
using BaseConverter;

namespace TestFilters.Controllers;

public class CatDto
{
    [JsonConverter(typeof(PandaJsonBaseConverterNotNullable))]
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}