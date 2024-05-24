using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.Entities;

[FilterModel(typeof(CategoryFilter))]
public class Category
{
    public long Id { get; set; }
    public List<CategoryType> Categories { get; set; } = null!;

    public List<Customer> Customers { get; set; } = null!;
    public List<CategoryName> CategoryNames { get; set; } = null!;
}