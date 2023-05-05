namespace PandaTech.EnumerableFilters;

public class FilterInfo
{
    public string PropertyName { get; set; } = null!;
    public List<ComparisonType> ComparisonTypes { get; set; }
    public string Table { get; set; } = null!;
}