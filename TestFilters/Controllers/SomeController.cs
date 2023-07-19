using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BaseConverter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.Mapper;
using static System.Linq.Expressions.Expression;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TestFilters.Controllers;

[ApiController]
[Route("[controller]")]
public class SomeController : ControllerBase
{
    private readonly Context _context;
    private readonly FilterProvider _filterProvider;
    private readonly Counter _counter;
    private readonly UpCounter2 _upCounter2;
    private readonly UpCounter _upCounter;
    private readonly IServiceProvider _serviceProvider;
    private readonly HttpClient _client;
    private readonly IMapping<Person, PersonDto> _personDtoMapper;

    public SomeController(Context context, Counter counter, UpCounter2 upCounter2, UpCounter upCounter,
        IServiceProvider serviceProvider, HttpClient client, IMapping<Person, PersonDto> personDtoMapper,
        FilterProvider filterProvider)
    {
        _context = context;
        _counter = counter;
        _upCounter2 = upCounter2;
        _upCounter = upCounter;
        _serviceProvider = serviceProvider;
        _client = client;
        _personDtoMapper = personDtoMapper;
        _filterProvider = filterProvider;

        _filterProvider.Add<PersonDto, Person>();


        _filterProvider.Add(
            new FilterProvider.Filter
            {
                SourceType = typeof(PersonDto),
                TargetType = typeof(Person),
                ComparisonTypes = new List<ComparisonType>
                {
                    ComparisonType.Equal, ComparisonType.In, ComparisonType.NotEqual
                },
                Converter = id => PandaBaseConverter.Base36ToBase10(id as string) ?? -1,
                DtoConverter = id => PandaBaseConverter.Base10ToBase36((long)id)!,
                SourcePropertyName = nameof(PersonDto.Id),
                TargetPropertyType = typeof(long),
                TargetPropertyName = nameof(Person.PersonId),
                SourcePropertyType = typeof(string)
            }
        );

        _filterProvider.Add(
            new FilterProvider.Filter
            {
                SourceType = typeof(PersonDto),
                TargetType = typeof(Person),
                ComparisonTypes = new List<ComparisonType>
                {
                    ComparisonType.Contains
                },
                Converter = id => _context.Cats.Find(id)!,
                DtoConverter = cat => new CatDto { Id = (cat as Cat)!.Id, Age = (cat as Cat)!.Age, Name = (cat as Cat)!.Name },
                SourcePropertyName = nameof(PersonDto.Cats),
                SourcePropertyType = typeof(int),
                TargetPropertyName = nameof(Person.Cats),
                TargetPropertyType = typeof(List<Cat>)
            }
        );


        _filterProvider.Add(
            new FilterProvider.Filter
            {
                SourceType = typeof(PersonDto),
                TargetType = typeof(Person),
                ComparisonTypes = new List<ComparisonType>
                {
                    ComparisonType.Contains, ComparisonType.In
                },
                Converter = id => id,
                DtoConverter = cat => cat,
                SourcePropertyName = nameof(PersonDto.Ints),
                SourcePropertyType = typeof(int),
                TargetPropertyName = nameof(Person.Ints),
                TargetPropertyType = typeof(List<int>)
            }
        );

        /*_filterProvider
            .For<PersonDto>()
            .SetDbType<Person>()
            .AutoMapFields()
            .AddFilter(nameof(PersonDto.Cats), nameof(Person.Cats))
            .WithComparrisonTypes(new[]{ ComparisonType.Contains });*/
    }

    [HttpGet("[action]")]
    public IActionResult getFilters()
    {
        return Ok(_filterProvider.GetFilterDtos<PersonDto>());
    }

    [HttpGet("[action]")]
    public IActionResult GetTables()
    {
        return Ok(_filterProvider.GetTables());
    }

    [HttpPost("[action]/{propertyName}")]
    public IActionResult Distinct([FromBody] GetDataRequest getDataRequest, [FromRoute] string propertyName)
    {
        var query = _context.Persons.Include(x => x.FavoriteCat).DistinctColumnValues(getDataRequest.Filters,
            propertyName, _filterProvider, 20, 1);

        return Ok(query);
    }


    [HttpGet("[action]")]
    public List<PersonDto> test1()
    {
        Expression<Func<Person, bool>> ex;


        return _context.Persons.Where("Name.StartsWith(@0)", "D").Take(10).AsEnumerable().Select(_personDtoMapper.Map)
            .ToList();
    }

    [HttpGet("[action]")]
    public List<PersonDto> test2()
    {
        var a = new List<long> { 1, 2, 3 };

        return _context.Persons.Where($"@0.Contains({nameof(Person.PersonId)})", a).Take(10).AsEnumerable()
            .Select(_personDtoMapper.Map).ToList();
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
        optionBuilder.UseNpgsql("Server=127.0.0.1;Database=xyz;Username=postgres;Password=example");

        var tasks = new List<Task>();
        var catId = 1;
        var context = new Context(optionBuilder.Options, _serviceProvider);
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
                IsHappy = Random.Shared.Next(0, 1) == 1,
                IsMarried = Random.Shared.Next(0, 3) == 0,
                IsWorking = Random.Shared.Next(0, 5) != 1,
                Ints = new List<int> { Random.Shared.Next(0, 50), Random.Shared.Next(0, 50), Random.Shared.Next(0, 50) },
            };

            for (var j = 0; j < catCount; j++)
            {
                person.Cats.Add(new Cat
                {
                    Id = catId++,
                    Name = NameProvider.GetRandomName(),
                    Age = Random.Shared.Next(1, 20),
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

    [HttpGet("[action]")]
    public IActionResult Count()
    {
        return Ok($"{_counter.Count()} {_upCounter.Count()} {_upCounter2.Count()}");
    }

    [HttpGet("[action]")]
    public IActionResult Count2()
    {
        _client.BaseAddress = new Uri("http://localhost/Some/Count");

        var response = _client.GetAsync("").Result;
        var content = response.Content.ReadAsStringAsync().Result;
        response.Dispose();

        response = _client.GetAsync("").Result;
        content += response.Content.ReadAsStringAsync().Result;
        response.Dispose();

        response = _client.GetAsync("").Result;
        content += response.Content.ReadAsStringAsync().Result;
        response.Dispose();

        response = _client.GetAsync("").Result;
        content += response.Content.ReadAsStringAsync().Result;
        response.Dispose();

        return Ok(content);
    }


    [HttpGet("[action]")]
    public IActionResult DT(DateTime date)
    {
        return Ok(date);
    }


    [HttpPost("persons/{page}/{pageSize}")]
    public List<PersonDto> GetPerson([FromBody] GetDataRequest request, int page, int pageSize)
    {
        return _context.GetPersons(request, page, pageSize, _filterProvider);
    }

    [HttpGet("persons/{page}/{pageSize}")]
    public List<PersonDto> GetPerson([FromQuery] string request, int page, int pageSize)
    {
        return _context.GetPersons(GetDataRequest.FromString(request), page, pageSize, _filterProvider);
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

    [HttpGet("GetPersons")]
    public FilteredDataResult<Person> GetPersons([FromQuery] string? filtersString, int page, int pageSize)
    {
        var now = DateTime.Now;
        var filters = JsonSerializer.Deserialize<GetDataRequest>(filtersString ?? "");

        if (filters == null)
        {
            return new FilteredDataResult<Person>();
        }

        var query = _context.Persons.ApplyFilters(filters.Filters, _filterProvider)
            .ApplyOrdering(filters.Order, _filterProvider);

        var response = new FilteredDataResult<Person>
        {
            Data = query.Include(p => p.Cats).Skip((page - 1) * pageSize).Take(pageSize)
                .ToList(),
            TotalCount = query.Count(),
            Aggregates = query.GetAggregates(filters.Aggregates)
        };
        return response;
    }
}

public class Phrase
{
    public string Text { get; set; } = null!;
    public DateTime Date { get; set; }

    [Key]
    public int Id { get; set; }
}

public class UpCounter
{
    private Counter _counter;

    public UpCounter(Counter counter)
    {
        _counter = counter;
    }

    public int Count() => _counter.Count();
}

public class UpCounter2
{
    private Counter _counter;

    public UpCounter2(Counter counter)
    {
        _counter = counter;
    }

    public int Count() => _counter.Count();
}