using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Dto.Public;

public class AggregateQueryRequest : FilterQueryRequest
{
    private AggregateType _aggregateType;

    public AggregateType AggregateType
    {
        get => _aggregateType;
        init
        {
            if (!Enum.IsDefined(typeof(AggregateType), value))
            {
                throw new AggregateTypeMissingException();
            }

            _aggregateType = value;
        }
    }

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

    public T CastAggregateTo<T>() where T : AggregateQueryRequest, new()
    {
        return new T
        {
            FilterQuery = FilterQuery,
            AggregateType = AggregateType,
            ColumnName = ColumnName
        };
    }
}