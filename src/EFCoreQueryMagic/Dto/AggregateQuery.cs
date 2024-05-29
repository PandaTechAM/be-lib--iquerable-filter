using EFCoreQueryMagic.Enums;

namespace EFCoreQueryMagic.Dto;

internal record AggregateQuery(string PropertyName, AggregateType AggregateType);
