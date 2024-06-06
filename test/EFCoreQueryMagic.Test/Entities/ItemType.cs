namespace EFCoreQueryMagic.Test.Entities;

public class ItemType : IEquatable<ItemType>
{
    public Guid Id { get; set; }
    public string NameAm { get; set; }
    public string NameRu { get; set; }
    public string NameEn { get; set; }

    public ICollection<ItemTypeMapping> ItemTypeMappings { get; set; }

    public bool Equals(ItemType? other)
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
        return Equals((ItemType)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}