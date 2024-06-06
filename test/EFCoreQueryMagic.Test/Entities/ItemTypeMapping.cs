using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(ItemFilter))]
public class ItemTypeMapping : IEquatable<ItemTypeMapping>
{
    public long Id { get; set; }
    public Guid ItemId { get; set; }
    public Guid ItemTypeId { get; set; }

    public Item Item { get; set; }
    public ItemType ItemType { get; set; }

    public bool Equals(ItemTypeMapping? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ItemTypeMapping)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}