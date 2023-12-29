using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters.Attributes;

namespace TestFilters.Controllers.Models;

[PrimaryKey(nameof(PersonId))]
[FilterModel(typeof(PersonDto))]
public class Person
{
    public long PersonId { get; set; }
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Sex Sex { get; set; }
    public int Age { get; set; }

    public Dummy? FavoriteCat { get; set; } = null!;
    public long? FavoriteCatId { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;

    public List<int> Ints { get; set; } = null!;
    public List<MyEnum> Enums { get; set; } = null!;
    public double Money { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsMarried { get; set; }
    public bool IsWorking { get; set; }
    public bool IsHappy { get; set; }
    public DateTime? NewBirthDate { get; set; }

    public List<Cat>? Cats { get; set; } = null!;
}