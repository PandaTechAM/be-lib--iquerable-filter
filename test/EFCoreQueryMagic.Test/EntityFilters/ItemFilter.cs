using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.EntityFilters;

[MappedToClass(typeof(Item))]
public class ItemFilter
{
    [MappedToProperty(nameof(Item.Id))] 
    public string Id { get; set; } = null!;
    
    [MappedToProperty(nameof(Item.IdNullable))]
    public string? IdNullable { get; set; }

    [MappedToProperty(nameof(Item.Price))]
    public double Price { get; set; }
    
    [MappedToProperty(nameof(Item.DiscountedPrice))]
    public double? DiscountedPrice { get; set; }
    
    [MappedToProperty(nameof(Item.MinPrice))]
    public float MinPrice { get; set; }
    
    [MappedToProperty(nameof(Item.MaxPrice))]
    public float? MaxPrice { get; set; }
    
    [MappedToProperty(nameof(Item.MinQuantity))]
    public short MinQuantity { get; set; }
    
    [MappedToProperty(nameof(Item.MaxQuantity))]
    public short? MaxQuantity { get; set; }
    
    [MappedToProperty(nameof(Item.UShort))]
    public ushort UShort { get; set; }
    
    [MappedToProperty(nameof(Item.UShortNullable))]
    public ushort? UShortNullable { get; set; }
    
    [MappedToProperty(nameof(Item.UInt))]
    public uint UInt { get; set; }
    
    [MappedToProperty(nameof(Item.UIntNullable))]
    public uint? UIntNullable { get; set; }

    [MappedToProperty(nameof(Item.ULong))]
    public ulong ULong { get; set; }
    
    [MappedToProperty(nameof(Item.UlongNullable))]
    public ulong? UlongNullable { get; set; }
    
    [MappedToProperty(nameof(Item.AvailablePeriod))]
    public TimeSpan AvailablePeriod { get; set; }
    
    [MappedToProperty(nameof(Item.UnavailablePeriod))]
    public TimeSpan? UnavailablePeriod { get; set; }
    
    [MappedToProperty(nameof(Item.DateTimeOffset))]
    public DateTimeOffset DateTimeOffset { get; set; }
    
    [MappedToProperty(nameof(Item.DateTimeOffsetNullable))]
    public DateTimeOffset? DateTimeOffsetNullable { get; set; }
    
    [MappedToProperty(nameof(Item.DateOnly))]
    public DateOnly DateOnly { get; set; }
    
    [MappedToProperty(nameof(Item.DateOnlyNullable))]
    public DateOnly? DateOnlyNullable { get; set; }
    
    [MappedToProperty(nameof(Item.TimeOnly))]
    public TimeOnly TimeOnly { get; set; }
    
    [MappedToProperty(nameof(Item.TimeOnlyNullable))]
    public TimeOnly? TimeOnlyNullable { get; set; }
    
    [MappedToProperty(nameof(Item.Char))]
    public char Char { get; set; }
    
    [MappedToProperty(nameof(Item.CharNullable))]
    public char? CharNullable { get; set; }
    
    [MappedToProperty(nameof(Item.Byte))]
    public byte Byte { get; set; }
    
    [MappedToProperty(nameof(Item.ByteNullable))]
    public byte? ByteNullable { get; set; }
    
    [MappedToProperty(nameof(Item.SByte))]
    public sbyte SByte { get; set; }
    
    [MappedToProperty(nameof(Item.SByteNullable))]
    public sbyte? SByteNullable { get; set; }

    [MappedToProperty(nameof(Item.OrderId), ConverterType = typeof(FilterPandaBaseConverter))]
    public long? OrderId { get; set; }

    [MappedToProperty(nameof(Item.ListString))] 
    public List<string> ListString { get; set; } = null!;
    
    [MappedToProperty(nameof(Item.ListStringNullable))]
    public List<string>? ListStringNullable { get; set; }
    
    public OrderFilter Order { get; set; } = null!;
}