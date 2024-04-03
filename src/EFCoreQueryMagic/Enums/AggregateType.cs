using System.Text.Json.Serialization;

namespace EFCoreQueryMagic.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AggregateType
{
    UniqueCount,
    Sum,
    Average,
    Min,
    Max
}