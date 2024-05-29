using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Dto.Public;

public class PageQueryRequest : FilterQueryRequest
{
    private int _page = 1;
    private int _pageSize = 10;

    public int Page
    {
        get => _page;
        set
        {
            if (value < 1)
            {
                throw new PaginationException("Page cannot be less than 1.");
            }

            _page = value;
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            _pageSize = value switch
            {
                < 1 => throw new PaginationException("Page size cannot be less than 1."),
                > 500 => throw new PaginationException("Page size cannot be greater than 500."),
                _ => value
            };
        }
    }
}