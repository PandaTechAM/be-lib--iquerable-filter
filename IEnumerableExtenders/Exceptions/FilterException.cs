namespace PandaTech.IEnumerableFilters.Exceptions;

public abstract class FilterException: ApplicationException
{
    protected FilterException(string message) : base(message)
    {
    }

    protected FilterException()
    {
        
    }
}