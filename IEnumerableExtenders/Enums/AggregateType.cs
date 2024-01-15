using System.Text.Json.Serialization;

namespace PandaTech.IEnumerableFilters.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    UniqueCount,
    Sum,
    Average,
    Min,
    Max
}