namespace PandaTech.IEnumerableFilters.Dto;

public class FilterInfo
{
    public string PropertyName { get; set; } = null!;
    public List<ComparisonType> ComparisonTypes { get; set; } = null!;
    public string Table { get; set; } = null!;
}