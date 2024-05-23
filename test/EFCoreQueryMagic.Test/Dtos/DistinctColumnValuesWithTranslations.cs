using BaseConverter.Attributes;

namespace EFCoreQueryMagic.Test.Dtos;

public class DistinctColumnValuesWithTranslations
{
    [PandaPropertyBaseConverter]
    public long? Id { get; set; }
    public string? EnglishUs { get; set; }
    public string? Russian { get; set; }
    public string? Armenian { get; set; }
}