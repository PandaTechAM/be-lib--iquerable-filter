using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Dto;
using TestFilters.Controllers.bulk;
using TestFilters.Controllers.Models;
using Random = System.Random;

namespace TestFilters.Controllers;

[ApiController]
[Route("[controller]")]
public class SomeController : ControllerBase
{
    private readonly Context _context;

    private readonly Aes256 _aes256;
    private readonly IServiceProvider _serviceProvider;

    public SomeController(Context context, IServiceProvider serviceProvider, Aes256 aes256)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _aes256 = aes256;
    }

    [HttpGet("[action]/{tableName}")]
    public IActionResult GetFilters(string tableName)
    {
        var type = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(x => x.Name == tableName &&
                                 x.CustomAttributes.Any(attr =>
                                     attr.AttributeType ==
                                     typeof(MappedToClassAttribute)));

        if (type is null)
            return NotFound();

        var properties = type.GetProperties().Where(x => x.CustomAttributes.Any(attr =>
            attr.AttributeType == typeof(MappedToPropertyAttribute))).ToList();
        var infos = properties.Select(
            x => new FilterInfo()
            {
                PropertyName = x.Name,
                Table = tableName,
                ComparisonTypes = (x.GetCustomAttribute<MappedToPropertyAttribute>()!.ComparisonTypes ?? new[]
                {
                    ComparisonType.Equal,
                    ComparisonType.NotEqual,
                    ComparisonType.In,
                    ComparisonType.NotIn
                }).ToList()
            }
        );


        return Ok(infos);
    }

    [HttpGet("[action]")]
    public IActionResult GetTables()
    {
        return Ok(Assembly.GetExecutingAssembly().GetTypes().Where(x =>
                x is { IsClass: true, IsAbstract: false } &&
                x.CustomAttributes.Any(attr => attr.AttributeType == typeof(MappedToClassAttribute)))
            .Select(x => x.Name)
            .ToList());
    }

    [HttpPost("[action]/{propertyName}")]
    public IActionResult Distinct([FromBody] GetDataRequest getDataRequest, [FromRoute] string propertyName)
    {
        var query = _context.Persons.Include(x => x.FavoriteCat).DistinctColumnValues<Person, PersonDto>(
            getDataRequest.Filters,
            propertyName, 20, 1, out var totalCount);

        return Ok(new { data = query, totalCount });
    }


    [HttpPost("[action]/{tableName}/{propertyName}")]
    public IActionResult Distinct([FromBody] GetDataRequest getDataRequest, [FromRoute] string tableName,
        [FromRoute] string propertyName)
    {
        var dtoType = Assembly.GetExecutingAssembly().GetTypes()
            .FirstOrDefault(x => x.Name == tableName &&
                                 x.CustomAttributes.Any(attr =>
                                     attr.AttributeType ==
                                     typeof(MappedToClassAttribute)));
        var dbType = dtoType!.GetCustomAttribute<MappedToClassAttribute>()!.TargetType!;

        var dbContextType = typeof(Context);
        var property = dbContextType.GetProperties().FirstOrDefault(
            x => x.PropertyType.IsGenericType &&
                 x.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                 x.PropertyType.GetGenericArguments()[0] == dbType
        );

        var instanceOfDbSet = property!.GetValue(_context);

        // this IQueryable<T> dbSet, List<FilterDto> filters, string columnName, int pageSize, int page, out long totalCount
        var methodDistinctColumnValues = typeof(EnumerableExtenders).GetMethods()
            .First(x => x.Name == "DistinctColumnValues").MakeGenericMethod(dbType, dtoType!);

        var totalCount = 0L;

        var paramList = new[] { instanceOfDbSet, getDataRequest.Filters, propertyName, 20, 1, totalCount };
        var list = methodDistinctColumnValues.Invoke(null, paramList);


        return Ok(new { data = list, totalCount });
    }

    [HttpPost("[action]")]
    public IActionResult PopulateDb()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        var start = DateTime.Now;

        const int count = 10_000;
        Console.Clear();

        var optionBuilder = new DbContextOptionsBuilder<Context>();
        optionBuilder.UseNpgsql("Server=127.0.0.1;Database=xyz;Username=test;Password=test");


        var tasks = new List<Task>();
        var catId = 1;
        var context = new Context(optionBuilder.Options, _serviceProvider);

        var catTypes = new List<CatTypes>
        {
            new() { Id = 1, Name = "Siam" },
            new() { Id = 2, Name = "MainKun" },
            new() { Id = 3, Name = "Tiger" }
        };

        context.AddRange(catTypes);
        context.SaveChanges();

        for (var i = 1; i <= count; i++)
        {
            var catCount = Random.Shared.Next(1, 4);
            var person = new Person
            {
                PersonId = i,
                Name = NameProvider.GetRandomName(),
                Age = Random.Shared.Next(15, 90),
                Cats = new List<Cat>(),
                Address = NameProvider.GetRandomAddress(),
                Email = "test@TEST.am",
                Sex = Enum.GetValues<Sex>()[i % 2],
                Money = Random.Shared.NextDouble() * 100000,
                Phone = "+37412345678",
                Surname = NameProvider.GetRandomName(),
                BirthDate = new DateTime(2000, 1, 1).AddDays(Random.Shared.Next(0, 10000)).ToUniversalTime(),
                IsHappy = Random.Shared.Next(0, 2) == 1,
                IsMarried = Random.Shared.Next(0, 3) == 0,
                IsWorking = Random.Shared.Next(0, 5) != 1,
                Ints = new List<int>
                    { Random.Shared.Next(0, 50), Random.Shared.Next(0, 50), Random.Shared.Next(0, 50) },
                Enums = new List<MyEnum>() { MyEnum.One, MyEnum.Two, MyEnum.Three },
            };

            for (var j = 0; j < catCount; j++)
            {
                var name = NameProvider.GetRandomName();

                person.Cats.Add(new Cat
                {
                    Id = catId++,
                    Name = name,
                    Age = Random.Shared.Next(1, 20),
                    SomeBytes = _aes256.Encrypt(name),
                    TypesId = Random.Shared.Next(1, 4)
                });
            }

            person.FavoriteCat = new Dummy();

            context.Add(person);

            if (i % 100 != 0) continue;
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(i);
            }
        }

        tasks.Add(context.SaveChangesAsync());

        Task.WaitAll(tasks.ToArray());

        Console.WriteLine(DateTime.Now - start);

        return Ok();
    }


    [HttpPost("persons/{page}/{pageSize}")]
    public List<PersonDto> GetPerson([FromBody] GetDataRequest request, int page, int pageSize)
    {
        return _context.GetPersons(request, page, pageSize);
    }

    [HttpGet("persons/{page}/{pageSize}")]
    public List<PersonDto> GetPerson([FromQuery] string request, int page, int pageSize)
    {
        return _context.GetPersons(GetDataRequest.FromString(request), page, pageSize);
    }


    [HttpPost("FilterDto")]
    public string FilterDto()
    {
        var request = new GetDataRequest()
        {
            Aggregates = new List<AggregateDto>()
            {
                new()
                {
                    AggregateType = AggregateType.Max,
                    PropertyName = "Age"
                }
            },
            Filters = new List<FilterDto>(),
        };

        return request.ToString();
    }

    [HttpPost("GetPersons")]
    public async Task<FilteredDataResult<Person>> GetPersons([FromBody] GetDataRequest filters, int page, int pageSize)
    {
        var query = _context.Persons.ApplyFilters<Person, PersonDto>(filters.Filters)
            .ApplyOrdering<Person, PersonDto>(filters.Order);

        var response = new FilteredDataResult<Person>
        {
            Data = query.Include(p => p.Cats).Skip((page - 1) * pageSize).Take(pageSize)
                .ToList(),
            TotalCount = query.Count(),
        };
        return response;
    }

    
    [HttpGet("column-values/{columnName}")]
    public async Task<DistinctColumnValuesResult> ColumnValues(string columnName, string filterString, int page,
        int pageSize)
    {
        var result = await _context.Cats
            .DistinctColumnValuesAsync<Cat, CatDto>(
                GetDataRequest.FromString(filterString).Filters,
                columnName,
                pageSize,
                page);

        return result;
    }
}