using System.Text.Json;
using System.Text.Json.Serialization;

namespace EFCoreQueryMagic.Dto;

internal record MagicQuery(List<FilterQuery> Filters, Ordering? Order)
{
    public static MagicQuery FromString(string value)
    {
        if (string.IsNullOrEmpty(value) || value == "{}")
        {
            return new MagicQuery([], null);
        }

        return JsonSerializer.Deserialize<MagicQuery>(value) ?? throw new Exception("Could not deserialize");
    }

    public override string ToString() => JsonSerializer.Serialize(this);
}