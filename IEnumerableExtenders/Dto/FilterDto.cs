using System.Text.Json.Serialization;

namespace PandaTech.IEnumerableFilters.Dto;

public class FilterDto // TODO: rename
{
    public string PropertyName { get; set; } = null!;
    public ComparisonType ComparisonType { get; set; }
    
    public List<object> Values { get; set; } = null!;
}

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

