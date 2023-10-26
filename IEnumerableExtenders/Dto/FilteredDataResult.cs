namespace PandaTech.IEnumerableFilters.Dto;

public class FilteredDataResult<T>
{
    public List<T> Data { get; set; } = null!;
    public long TotalCount { get; set; }
    public Dictionary<string, object?> Aggregates { get; set; } = null!;
}