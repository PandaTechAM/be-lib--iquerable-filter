using PandaTech.EnumerableFilters.Dtos;

namespace PandaTech.EnumerableFilters;

public class GetDataRequest
{
    public List<FilterDto> Filters { get; set; } = null!;
    public List<AggregateDto> Aggregates { get; set; } = null!;
    
    public Ordering OrderBy { get; set; } = null!;
    
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class Ordering
{
    public string PropertyName { get; set; } = null!;
    public bool IsDescending { get; set; }
}

