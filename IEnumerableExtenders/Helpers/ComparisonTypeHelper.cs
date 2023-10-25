using PandaTech.IEnumerableFilters.Dto;

namespace PandaTech.IEnumerableFilters.Helpers;

public enum ComparisonTypesDefault
{
    Numeric,
    String,
    DateTime,
    Bool,
    Guid,
    Enum
}

public static class DefaultComparisonTypes
{
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
    };

    public static readonly ComparisonType[] String =
    {
        ComparisonType.In,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
        ComparisonType.Contains,
        ComparisonType.StartsWith,
        ComparisonType.EndsWith,
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
    };

    public static readonly ComparisonType[] Bool =
    {
        ComparisonType.Equal,
        ComparisonType.NotEqual,
        ComparisonType.IsTrue,
        ComparisonType.IsFalse
    };

    public static readonly ComparisonType[] Guid =
    {
        ComparisonType.In,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
    };
    
    public static readonly ComparisonType[] Enum =
    {
        ComparisonType.In,
        ComparisonType.Equal,
        ComparisonType.NotEqual,
    };
}