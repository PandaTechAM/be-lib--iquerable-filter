namespace PandaTech.EnumerableFilters.Dtos;

public class FilteredDataResult<T>
{
    public List<T> Data { get; set; } = null!;
    public int TotalCount { get; set; }
    
    public Dictionary<string, object?> Aggregates { get; set; } = null!;
}