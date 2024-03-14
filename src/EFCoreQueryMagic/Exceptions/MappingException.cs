namespace EFCoreQueryMagic.Exceptions;

public class MappingException : FilterException
{
    public MappingException(string message) : base(message)
    {
    }

    public MappingException()
    {
    }
}