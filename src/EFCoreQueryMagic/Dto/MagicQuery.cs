using System.Text.Json;

namespace EFCoreQueryMagic.Dto;

internal record MagicQuery(List<FilterQuery> Filters, Ordering? Order)
{
    public static MagicQuery FromString(string value) => value == string.Empty
        ? new MagicQuery([], null)
        : JsonSerializer.Deserialize<MagicQuery>(value) ?? throw new Exception("Could not deserialize");

    public override string ToString() => JsonSerializer.Serialize(this);
}