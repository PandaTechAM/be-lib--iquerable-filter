namespace EFCoreQueryMagic.Exceptions;

public class UnsupportedFilterException : FilterException
{
    public UnsupportedFilterException(string s) : base(s)
    {
    }
}