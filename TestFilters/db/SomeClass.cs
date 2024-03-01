namespace TestFilters.db;

public class SomeClass
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? NullableString { get; set; }
    public byte[] NameEncrypted { get; set; }
}