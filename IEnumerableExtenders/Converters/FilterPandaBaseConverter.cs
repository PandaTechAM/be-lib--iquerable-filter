using BaseConverter;

namespace PandaTech.IEnumerableFilters.Converters;

public class FilterPandaBaseConverter : IConverter<string, long>
{
    public long ConvertTo(string from)
    {
        return PandaBaseConverter.Base36ToBase10(from)!.Value;
    }

    public string ConvertFrom(long from)
    {
        return PandaBaseConverter.Base10ToBase36(from)!;
    }
}

public class FilterNullablePandaBaseConverter : IConverter<string?, long?>
{
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