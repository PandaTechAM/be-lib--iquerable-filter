using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Enums;

namespace TestFilters.db;

public class CompanyFilter
{
    [MappedToProperty(nameof(Company.Id))]
    [Order(2)]
    public long Id { get; set; }

    [MappedToProperty(nameof(Company.Age))]
    [Order(direction: OrderDirection.Descending)]
    public long Age { get; set; }
    
    [MappedToProperty(nameof(Company.NullableString))]
    public string? NullableString { get; set; }

    [MappedToProperty(nameof(Company.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Company.Type))]
    public string Type { get; set; } = null!;

    [MappedToProperty(nameof(Company.Types))]
    public string Types { get; set; } = null!;

    [MappedToProperty(nameof(Company.IsEnabled))]
    public bool IsEnabled { get; set; }

    [MappedToProperty(nameof(Company.NameEncrypted), Encrypted = true, Sortable = false)]
    public string NameEncrypted { get; set; } = null!;

    [MappedToProperty(nameof(Company.Info), nameof(Company.Info.Name))]
    public string InfoName { get; set; } = null!;
}