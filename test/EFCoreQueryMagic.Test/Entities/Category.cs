using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(CategoryFilter))]
public class Category: IEquatable<Category>
{
    public long Id { get; set; }
    public List<CategoryType> Categories { get; set; } = null!;

    public List<Customer> Customers { get; set; } = null!;
    public List<CategoryName> CategoryNames { get; set; } = null!;

    public bool Equals(Category? other)
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
        return Equals((Category)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}