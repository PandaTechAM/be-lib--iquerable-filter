namespace EFCoreQueryMagic.Dto.Public;

public class ColumnDistinctValueQueryRequest : PageQueryRequest
{
    public string ColumnName { get; init; }
}