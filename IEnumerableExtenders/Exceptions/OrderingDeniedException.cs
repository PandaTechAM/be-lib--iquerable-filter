namespace PandaTech.IEnumerableFilters.Exceptions;

public class OrderingDeniedException : FilterException
{
    public OrderingDeniedException(string message) : base(message)
    {
    }

    public OrderingDeniedException()
    {
    }
}