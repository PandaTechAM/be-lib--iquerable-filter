using System.Text.Json;
using EFCoreQueryMagic.Exceptions;

namespace EFCoreQueryMagic.Dto.Public;

public record MagicQuery(List<FilterQuery> Filters, Ordering? Order)
{
    public static MagicQuery FromString(string value)
    {
        if (string.IsNullOrEmpty(value) || value == "{}")
        {
            return new MagicQuery([], null);
        }

        try
        {
            return JsonSerializer.Deserialize<MagicQuery>(value) ?? throw new Exception("Could not deserialize");
        }
        catch (Exception e)
        {
            throw new InvalidJsonException();
        }
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}