namespace PandaTech.IEnumerableFilters.Dto;

public class DistinctColumnValuesResult
{
    public List<object> Values { get; set; } = null!;
    public long TotalCount { get; set; }
}