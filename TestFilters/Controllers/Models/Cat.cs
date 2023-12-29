using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;

namespace TestFilters.Controllers.Models;

[PrimaryKey(nameof(Id))]
public class Cat
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }

    public byte[] SomeBytes { get; set; } = null!;

    public CatTypes Types { get; set; } = null!;
    public long TypesId { get; set; }

    public override string ToString() => $"Cat {Name} {Age} {Id}";
}