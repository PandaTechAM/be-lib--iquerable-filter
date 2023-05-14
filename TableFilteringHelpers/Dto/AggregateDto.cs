using System.Text.Json.Serialization;

namespace TableFilteringHelpers.Dto;

public class AggregateDto
{
    public string PropertyName { get; set; } = null!;
    public AggregateType AggregateType { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    UniqueCount,
    Sum,
    Average,
    Min,
    Max
}