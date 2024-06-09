namespace EFCoreQueryMagic.Dto.Public;

public class FilterQueryRequest
{
    public string FilterQuery { get; set; } = "{}";
    
    public T CastFilterTo<T>() where T : FilterQueryRequest, new()
    {
        return new T
        {
            FilterQuery = FilterQuery
        };
    }
}
