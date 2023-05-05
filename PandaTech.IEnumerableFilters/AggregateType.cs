using System.Text.Json.Serialization;

namespace PandaTech.EnumerableFilters;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    UniqueCount,
    Sum,
    Average,
    Min,
    Max
}