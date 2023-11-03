namespace PandaTech.IEnumerableFilters.Exceptions;

public class UnsupportedFilterException : FilterException
{
    public UnsupportedFilterException(string s) : base(s)
    {
    }
}