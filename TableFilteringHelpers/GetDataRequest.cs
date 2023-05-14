using System.Text.Json;
using TableFilteringHelpers.Dto;

namespace TableFilteringHelpers;

public class GetDataRequest
{
    public List<FilterDto> Filters { get; set; } = null!;
    public List<AggregateDto> Aggregates { get; set; } = null!;

    public static GetDataRequest FromString(string value) => JsonSerializer.Deserialize<GetDataRequest>(value) ?? throw new Exception("Could not deserialize");
   
    public override string ToString() => JsonSerializer.Serialize(this);
}


