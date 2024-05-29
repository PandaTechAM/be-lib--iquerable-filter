using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto.Public;

public class AggregateQueryRequest : FilterQueryRequest
{
    public AggregateType AggregateType { get; init; }
    public string ColumnName { get; init; }
}