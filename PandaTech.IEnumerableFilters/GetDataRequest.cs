using PandaTech.EnumerableFilters.Dtos;

namespace PandaTech.EnumerableFilters;

public class GetDataRequest
{
    public List<FilterDto> Filters { get; set; } = null!;
    public List<AggregateDto> Aggregates { get; set; } = null!;
}


