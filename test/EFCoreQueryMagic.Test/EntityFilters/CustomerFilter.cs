using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Entities;

namespace EFCoreQueryMagic.Test.EntityFilters;

[MappedToClass(typeof(Customer))]
public class CustomerFilter
{
    [MappedToProperty(nameof(Customer.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    public int Id { get; set; }

    [MappedToProperty(nameof(Customer.Name), ConverterType = typeof(EncryptedConverter))]
    public byte[] Name { get; set; } = null!;

    [MappedToProperty(nameof(Customer.Email))]
    public string Email { get; set; } = null!;

    [MappedToProperty(nameof(Customer.CategoryId), ConverterType = typeof(FilterPandaBaseConverter))]
    public int CategoryId { get; set; }

    public CategoryFilter Category { get; set; } = null!;
}