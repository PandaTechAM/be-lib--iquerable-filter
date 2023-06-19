using Microsoft.EntityFrameworkCore;

namespace TestFilters.Controllers;

[PrimaryKey(nameof(Id))]
public class Cat
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}