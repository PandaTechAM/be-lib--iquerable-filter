using Microsoft.EntityFrameworkCore;

namespace TestFilters.Controllers;

[PrimaryKey(nameof(Id))]
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int Age { get; set; }
    
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
    
    public double Money { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsMarried { get; set; }
    public bool IsWorking { get; set; }
    public bool IsHappy { get; set; }
    
    public List<Cat> Cats { get; set; } = null!;
}