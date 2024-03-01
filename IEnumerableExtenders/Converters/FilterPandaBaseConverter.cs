using BaseConverter;
using Microsoft.EntityFrameworkCore;

namespace PandaTech.IEnumerableFilters.Converters;

public class FilterPandaBaseConverter : IConverter<string, long>
{
    public DbContext Context { get; set; }

    public long ConvertTo(string from)
    {
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }

    public string ConvertFrom(long from)
    {
        return PandaBaseConverter.Base10ToBase36(from)!;
    }
}