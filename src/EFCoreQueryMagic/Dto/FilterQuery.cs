using System.Text.Json;
using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto;

internal class FilterQuery
{
    public string PropertyName { get; set; } = null!;
    public ComparisonType ComparisonType { get; set; }
    public List<object?> Values { get; set; }

    public override string ToString() => JsonSerializer.Serialize(new FilterQueryWrapper { Filters = [this] });
}

internal class FilterQueryWrapper
{
    public List<FilterQuery> Filters { get; set; }
}