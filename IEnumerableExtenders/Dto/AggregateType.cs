using System.Text.Json.Serialization;

namespace PandaTech.IEnumerableFilters.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    UniqueCount,
    Sum,
    Average,
    Min,
    Max
}