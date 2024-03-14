using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;

namespace Tests;

public class SomeConverter : IConverter<string, int>
{
    public int Convert(string from)
    {
        return int.Parse(from);
    }

    public DbContext Context { get; set; }

    public int ConvertTo(string from)
    {
        return int.Parse(from);
    }

    public string ConvertFrom(int to)
    {
        return to.ToString();
    }
}