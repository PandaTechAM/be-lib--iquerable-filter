namespace PandaTech.IEnumerableFilters;

public class DirectConverter : IConverter<object, object>
{
    public object ConvertTo(object from)
    {
        return from;
    }

    public object ConvertFrom(object to)
    {
        return to;
    }
}