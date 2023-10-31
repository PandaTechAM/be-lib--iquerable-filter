using Microsoft.EntityFrameworkCore;

namespace TestFilters.Controllers.Models;

[PrimaryKey(nameof(Id))]
public class Cat
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }

    public byte[] SomeBytes { get; set; } = null!;
    
    public CatTypes Types { get; set; } = null!;
    
    public override string ToString() => $"Cat {Name} {Age} {Id}";
    
}

public class CatTypes
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
}