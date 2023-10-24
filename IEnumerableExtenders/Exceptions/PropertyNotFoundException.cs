namespace PandaTech.IEnumerableFilters.Exceptions;

public class PropertyNotFoundException : FilterException
{
    public PropertyNotFoundException(string message) : base(message)
    {
    }
    
    public PropertyNotFoundException()
    {
        
    }
}