namespace PandaTech.IEnumerableFilters.Dto;

public class FilterDto // TODO: rename
{
    public string PropertyName { get; set; } = null!;
    public ComparisonType ComparisonType { get; set; }
    
    public List<object> Values { get; set; } = null!;
}