namespace PandaTech.IEnumerableFilters.Exceptions;

public class ComparisonNotSupportedException : FilterException
{
    public ComparisonNotSupportedException(string message) : base(message)
    {
    }
    
    public ComparisonNotSupportedException()
    {
        
    }

    public ComparisonNotSupportedException(FilterProvider.FilterKey key) : base(
        $"Comparison {key.ComparisonType} not supported for type {key.TargetType}")
    {
    }
}