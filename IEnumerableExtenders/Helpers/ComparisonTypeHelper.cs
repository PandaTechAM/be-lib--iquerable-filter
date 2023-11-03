using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters.Helpers;

public enum ComparisonTypesDefault
{
    Numeric,
    String,
    DateTime,
    Bool,
    Guid,
    Enum,
    ByteArray
}

public static class DefaultComparisonTypes
{
    public static readonly ComparisonType[] Default =
    {
        ComparisonType.In,
        ComparisonType.NotIn,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
    };

    public static readonly ComparisonType[] ByteArray =
    {
        ComparisonType.In,
        ComparisonType.NotIn,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
    };

    public static readonly ComparisonType[] Numeric =
    {
        ComparisonType.In,
        ComparisonType.Between,
        ComparisonType.Equal,
        ComparisonType.GreaterThan,
        ComparisonType.GreaterThanOrEqual,
        ComparisonType.LessThan,
        ComparisonType.LessThanOrEqual,
        ComparisonType.NotEqual,
        ComparisonType.IsEmpty,
        ComparisonType.IsNotEmpty
    };

    public static readonly ComparisonType[] String =
    {
        ComparisonType.In,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
        ComparisonType.Contains,
        ComparisonType.NotContains,
        ComparisonType.StartsWith,
        ComparisonType.EndsWith,
        ComparisonType.IsEmpty,
        ComparisonType.IsNotEmpty
    };

    public static readonly ComparisonType[] DateTime =
    {
        ComparisonType.In,
        ComparisonType.Between,
        ComparisonType.Equal,
        ComparisonType.GreaterThan,
        ComparisonType.GreaterThanOrEqual,
        ComparisonType.LessThan,
        ComparisonType.LessThanOrEqual,
        ComparisonType.NotEqual,
        ComparisonType.IsEmpty,
        ComparisonType.IsNotEmpty
    };

    public static readonly ComparisonType[] Bool =
    {
        ComparisonType.IsTrue,
        ComparisonType.IsFalse
    };

    public static readonly ComparisonType[] Guid =
    {
        ComparisonType.Equal,
        ComparisonType.NotEqual,
        ComparisonType.In,
        ComparisonType.IsEmpty,
        ComparisonType.IsNotEmpty
    };

    public static readonly ComparisonType[] Enum =
    {
        ComparisonType.In,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
        ComparisonType.IsEmpty,
        ComparisonType.IsNotEmpty
    };
}