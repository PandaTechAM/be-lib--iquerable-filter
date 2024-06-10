using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Dto.Public;

public class ColumnDistinctValueQueryRequest : PageQueryRequest
{
    private string _columnName = null!;
    public string ColumnName
    {
        get => _columnName;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ColumnNameMissingException();
            }
            _columnName = value;
        }
    }

    public T CastColumnNameTo<T>() where T : ColumnDistinctValueQueryRequest, new()
    {
        return new T
        {
            Page = Page,
            PageSize = PageSize,
            FilterQuery = FilterQuery,
            ColumnName = ColumnName
        };
    }
}