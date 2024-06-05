using BaseConverter.Attributes;

namespace EFCoreQueryMagic.Test.Dtos;

public class DistinctColumnValuesWithTranslations: IEquatable<DistinctColumnValuesWithTranslations>
{
    [PandaPropertyBaseConverter]
    public long? Id { get; set; }
    public string? EnglishUs { get; set; }
    public string? Russian { get; set; }
    public string? Armenian { get; set; }


    public int CompareTo(DistinctColumnValuesWithTranslations? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return string.Compare(EnglishUs, other.EnglishUs, StringComparison.Ordinal);
    }

    public bool Equals(DistinctColumnValuesWithTranslations? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EnglishUs == other.EnglishUs && Russian == other.Russian && Armenian == other.Armenian;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DistinctColumnValuesWithTranslations)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EnglishUs, Russian, Armenian);
    }
}