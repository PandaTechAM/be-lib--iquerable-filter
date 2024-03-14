namespace EFCoreQueryMagic.Exceptions;

public abstract class FilterException : Exception
{
    protected FilterException(string message) : base(message)
    {
    }

    protected FilterException()
    {
    }
}