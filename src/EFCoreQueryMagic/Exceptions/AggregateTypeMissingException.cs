namespace EFCoreQueryMagic.Exceptions;

public class AggregateTypeMissingException(string message = "Aggregate type is missing or not in range")
    : FilterException(message);