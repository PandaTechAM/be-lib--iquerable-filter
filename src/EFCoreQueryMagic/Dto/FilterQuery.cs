using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto;

internal class FilterQuery
{
    public string PropertyName { get; set; } = null!;
    public ComparisonType ComparisonType { get; set; }
    public List<object?> Values { get; set; }
}