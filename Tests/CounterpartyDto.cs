using PandaTech.IEnumerableFilters.Attributes;

namespace Tests;

[MappedToClass(typeof(Counterparty))]
public class CounterpartyDto
{
    [MappedToProperty(nameof(Counterparty.Id), ConverterType = typeof(SomeConverter))]
    public string Id { get; set; }

    [MappedToProperty(nameof(Counterparty.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Counterparty.Surname))]
    public string Surname { get; set; } = null!;

    [MappedToProperty(nameof(Counterparty.Email))]
    public string Email { get; set; } = null!;

    [MappedToProperty(nameof(Counterparty.Age))]
    public int Age { get; set; }

    [MappedToProperty(nameof(Counterparty.Tags))]
    public List<Tag> Tags { get; set; } = null!;
}