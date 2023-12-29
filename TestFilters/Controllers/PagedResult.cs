namespace TestFilters.Controllers;

public class PagedResult<T>
{
    public List<T> Data { get; set; } = null!;
}