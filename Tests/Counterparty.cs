namespace Tests;

public class Counterparty
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    public List<CounterpartyTags> Tags { get; set; } = null!;
}