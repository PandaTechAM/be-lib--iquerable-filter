using EFCoreQueryMagic.Attributes;

namespace EFCoreQueryMagic.Test.Enums;

public enum PaymentStatus
{
    Pending,
    Completed,
    [HideEnumValue]
    Failed
}