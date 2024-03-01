using BaseConverter;
using Microsoft.EntityFrameworkCore;

namespace PandaTech.IEnumerableFilters.Converters;

public class FilterNullablePandaBaseConverter : IConverter<string?, long?>
{
    public DbContext Context { get; set; }

    public long? ConvertTo(string? from)
    {
        if (from is null) return null;
        
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }

    public string? ConvertFrom(long? from)
    {
        if (from is null) return null;

        return PandaBaseConverter.Base10ToBase36(from)!;
    }
}