using BaseConverter;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Converters;

public class FilterPandaBaseConverter : IConverter<string?, long?>
{
    public DbContext Context { get; set; } = null!;

    public long? ConvertTo(string? from)
    {
        return PandaBaseConverter.Base36ToBase10(from);
    }

    public string? ConvertFrom(long? from)
    {
        return PandaBaseConverter.Base10ToBase36(from);
    }
}