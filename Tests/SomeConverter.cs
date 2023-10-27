using PandaTech.IEnumerableFilters;

namespace Tests;

public class SomeConverter : IConverter<string, int>
{
    public int Convert(string from)
    {
        return int.Parse(from);
    }

    public int ConvertTo(string from)
    {
        return int.Parse(from);
    }

    public string ConvertFrom(int to)
    {
        return to.ToString();
    }
}