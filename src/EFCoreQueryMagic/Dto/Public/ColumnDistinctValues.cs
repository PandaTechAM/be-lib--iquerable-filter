namespace EFCoreQueryMagic.Dto.Public;

public class ColumnDistinctValues
{
    public List<object> Values { get; set; } = [];
    public long TotalCount { get; set; }

    public override string ToString()
    {
        return $"{nameof(Values)}: {string.Join(';', Values)}, {nameof(TotalCount)}: {TotalCount}";
    }
}