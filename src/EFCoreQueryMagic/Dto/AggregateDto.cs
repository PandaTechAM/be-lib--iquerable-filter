using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto;

public class AggregateDto
{
    public string PropertyName { get; set; } = null!;
    public AggregateType AggregateType { get; set; }
}