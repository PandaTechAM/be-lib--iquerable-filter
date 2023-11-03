namespace PandaTech.IEnumerableFilters.Exceptions;

public class ComparisonNotSupportedException : FilterException
{
    public ComparisonNotSupportedException(string message) : base(message)
    {
    }

    public ComparisonNotSupportedException()
    {
    }
}