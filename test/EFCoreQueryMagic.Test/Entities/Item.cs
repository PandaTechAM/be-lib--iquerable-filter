using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(ItemFilter))]
public class Item : IEquatable<Item>
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

    public List<string> ListString { get; set; } = [];

    public List<string>? ListStringNullable { get; set; }

    public long? OrderId { get; set; }
    public Order? Order { get; set; }
    
    public ICollection<ItemTypeMapping> ItemTypeMappings { get; set; }

    public bool Equals(Item? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Item)obj);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Id);
        hashCode.Add(IdNullable);
        hashCode.Add(Price);
        hashCode.Add(DiscountedPrice);
        hashCode.Add(MinPrice);
        hashCode.Add(MaxPrice);
        hashCode.Add(MinQuantity);
        hashCode.Add(MaxQuantity);
        hashCode.Add(UShort);
        hashCode.Add(UShortNullable);
        hashCode.Add(UInt);
        hashCode.Add(UIntNullable);
        hashCode.Add(ULong);
        hashCode.Add(UlongNullable);
        hashCode.Add(Char);
        hashCode.Add(CharNullable);
        hashCode.Add(Byte);
        hashCode.Add(ByteNullable);
        hashCode.Add(SByte);
        hashCode.Add(SByteNullable);
        hashCode.Add(AvailablePeriod);
        hashCode.Add(UnavailablePeriod);
        hashCode.Add(DateTimeOffset);
        hashCode.Add(DateTimeOffsetNullable);
        hashCode.Add(DateOnly);
        hashCode.Add(DateOnlyNullable);
        hashCode.Add(TimeOnly);
        hashCode.Add(TimeOnlyNullable);
        hashCode.Add(CreatedAt);
        hashCode.Add(ListString);
        hashCode.Add(ListStringNullable);
        hashCode.Add(OrderId);
        hashCode.Add(Order);
        return hashCode.ToHashCode();
    }
}