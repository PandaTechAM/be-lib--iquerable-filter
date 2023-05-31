using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;

namespace TestFilters.Controllers;

[PrimaryKey(nameof(Id))]
public class Cat
{
    [FilterValueConverter<string, long>(typeof(TestConverter))]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}