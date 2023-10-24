using PandaTech.IEnumerableFilters.Attributes;

namespace Tests;

[MappedToClass(typeof(Counterparty))]
public class CounterpartyShortDto
{
    [MappedToProperty(nameof(Counterparty.Id))]
    public string Id { get; set; }

    [MappedToProperty(nameof(Counterparty.Name))]
    public string Name { get; set; } = null!;
}