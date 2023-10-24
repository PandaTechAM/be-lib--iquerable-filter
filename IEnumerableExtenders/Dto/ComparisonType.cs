using System.Text.Json.Serialization;

namespace PandaTech.IEnumerableFilters.Dto;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ComparisonType
{
    Equal,
    NotEqual,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    In,
    NotIn,
    IsNotEmpty,
    IsEmpty,
    Between,
    NotContains,
    HasCountEqualTo,
    HasCountBetween,
    IsTrue,
    IsFalse
}