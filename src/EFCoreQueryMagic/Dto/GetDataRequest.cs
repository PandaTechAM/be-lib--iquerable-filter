using System.Text.Json;

namespace EFCoreQueryMagic.Dto;

public class GetDataRequest
{
    public List<FilterDto> Filters { get; set; } = new();
    public List<AggregateDto> Aggregates { get; set; } = new();

    public Ordering Order { get; set; } = new();

    public static GetDataRequest FromString(string value) => value == string.Empty
        ? new GetDataRequest()
        : JsonSerializer.Deserialize<GetDataRequest>(value) ?? throw new Exception("Could not deserialize");

    public override string ToString() => JsonSerializer.Serialize(this);
}