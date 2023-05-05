using Microsoft.AspNetCore.Mvc;
using PandaTech.IQueryableFilter;

namespace Test.Controllers;

[ApiController]
[Route("[controller]")]
public class MyController : ControllerBase
{
    [HttpGet("GetSome")]
    public List<DTOSome> GetSome( List<DTOFilter> dtoFilters)
    {

        var filters = dtoFilters.Select(filter => new Filter<Some>()).ToList();
        
        return DTOSome.GetData<Some, DTOSome>(filters);
    }


    [HttpGet("GetFilters")]
    public List<FilterData> GetFilters(string DTOType)
    {
        var type = Type.GetType("Test.Controllers."+DTOType);

        var a = Filterable.GetFilters<DTOSome>();
        
        return (List<FilterData>)type.GetMethods().FirstOrDefault(m => m.Name == "GetFilters")?.Invoke(null, null) ?? new List<FilterData>();
    }
}

public class DTOFilter 
{
    ComparisonType ComparisonType { get; set; }
    string PropertyName { get; set; } = null!;
    List<object> Values { get; set; } = null!;
}

public class DTOSome: Filterable
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? SomeNullable { get; set; }
    public int SomeInt { get; set; }
    public long AnotherSomeId { get; set; }
    public string AnotherSomeName { get; set; } = null!;
}

public class Context 
{
    public List<Some> Some { get; set; }
    public List<T> Entity<T>() where T: class
    {
        return new();
    }
    
    Context()
    {
        Some = new List<Some>();
        
        for (int i = 0; i < 100; i++)
        {
            Some.Add(new Some
            {
                Id = i,
                Name = $"Name {i}",
                Description = $"Description {i}",
                SomeNullable = i % 2 == 0 ? $"SomeNullable {i}" : null,
                SomeInt = i,
                AnotherSome = new AnotherSome
                {
                    Id = i,
                    Name = $"AnotherSome Name {i}"
                }
            });
        }
    }
}

public class Some
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? SomeNullable { get; set; }
    public int SomeInt { get; set; }
    public AnotherSome AnotherSome { get; set; } = null!;
}

public class AnotherSome
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;   
}