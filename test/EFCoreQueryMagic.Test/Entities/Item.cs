using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(ItemFilter))]
public class Item
{
    public Guid Id { get; set; }
    public Guid? IdNullable { get; set; }
    public double Price { get; set; }
    public double? DiscountedPrice { get; set; }
    
    public float MinPrice { get; set; }
    public float? MaxPrice { get; set; }
    
    public short MinQuantity { get; set; }
    public short? MaxQuantity { get; set; }
    
    public ushort UShort { get; set; }
    public ushort? UShortNullable { get; set; }
    
    public uint UInt { get; set; }
    public uint? UIntNullable { get; set; }
    
    public ulong ULong { get; set; }
    public ulong? UlongNullable { get; set; }
    
    public char Char { get; set; }
    public char? CharNullable { get; set; }
    
    public byte Byte { get; set; }
    public byte? ByteNullable { get; set; }
    
    public sbyte SByte { get; set; }
    public sbyte? SByteNullable { get; set; }

    public TimeSpan AvailablePeriod { get; set; }
    public TimeSpan? UnavailablePeriod { get; set; }
    
    public DateTimeOffset DateTimeOffset { get; set; }
    public DateTimeOffset? DateTimeOffsetNullable { get; set; }
    
    public DateOnly DateOnly { get; set; }
    public DateOnly? DateOnlyNullable { get; set; }
    
    public TimeOnly TimeOnly { get; set; }
    public TimeOnly? TimeOnlyNullable { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public long OrderId { get; set; }
    public Order Order { get; set; }
}