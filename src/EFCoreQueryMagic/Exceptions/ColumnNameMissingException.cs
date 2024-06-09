namespace EFCoreQueryMagic.Exceptions;

public class ColumnNameMissingException(string message = "Column name missing") : FilterException(message);