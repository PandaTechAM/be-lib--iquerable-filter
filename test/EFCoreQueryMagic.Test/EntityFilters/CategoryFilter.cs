using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;

namespace EFCoreQueryMagic.Test.EntityFilters;

[MappedToClass(typeof(Category))]
public class CategoryFilter
{
    [MappedToProperty(nameof(Category.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    public int Id { get; set; }

    [MappedToProperty(nameof(Category.Categories))]
    public List<CategoryType> Categories { get; set; } = null!;

    public List<CustomerFilter> Customers { get; set; } = null!;
}