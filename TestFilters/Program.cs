using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;
using PandaTech.IEnumerableFilters.Converters;
using PandaTech.IEnumerableFilters.Dto;
using PandaTech.IEnumerableFilters.Extensions;
using TestFilters;
using TestFilters.Components;
using TestFilters.db;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<PostgresContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(
        "Host=localhost;Database=filter_test;Username=test;Password=test"
    ));

// base64 encoded 32 byte key
var key = "QXNkZmdoamtsbW5vcHFyc3R1dnd4eXo0NDU2Nzg5MjE=";

builder.Services.AddPandatechCryptoAes256(options => options.Key = key);


builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

EncryptedConverter.Aes256 = app.Services.GetRequiredService<Aes256>();

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
await db.Populate(1000);
await db.PopulateTest();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/generate/{count:int}", (PostgresContext context, int count) => context.Populate(count));
app.MapGet("/api/companies", (PostgresContext context, [FromQuery] int page, [FromQuery] int pageSize,
    [FromQuery] string q) => S.Companies(context, page, pageSize, q));
app.MapGet("/api/companies/distinct/{columnName}", (PostgresContext context, [FromRoute] string columnName,
        [FromQuery] string filterString, [FromQuery] int page, [FromQuery] int pageSize) =>
    S.DistinctColumnValues(context, columnName, filterString, page, pageSize));


app.MapGet("/api/test/distinct", S.DistinctTest);
app.MapGet("/api/test/direct", S.DirectTest);
app.MapGet("/api/test/join", S.JoinTest);

app.MapGet("/{Name}/{foo}", Bar);

app.Run();

void Bar ([FromQuery] Foo foo)
{
    Console.WriteLine(foo.Name);
    Console.WriteLine(foo.page);
    Console.WriteLine(foo.pageSize);
}
class Foo: IParsable<Foo>
{
    public string Name { get; set; }
    public int page { get; set; }
    public int pageSize { get; set; }
    
    
    public static Foo Parse(string s, IFormatProvider? provider)
    {
        Console.WriteLine(s);
        return default;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Foo result)
    {
        Console.WriteLine(s);
        result = default;
        return false;
    }
}

namespace TestFilters
{
    class S
    {
        public static async Task<List<Company>> Companies(PostgresContext context, [FromQuery] int page,
            [FromQuery] int pageSize,
            [FromQuery] string q)
        {
            var req = GetDataRequest.FromString(q);

            return await context.Companies
                .ApplyFilters(req.Filters, context)
                .Include(x => x.SomeClass)
                .ApplyOrdering(req.Order)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public static async Task<DistinctColumnValuesResult> DistinctColumnValues(PostgresContext context,
            [FromQuery] string columnName,
            [FromQuery] string filterString, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var req = GetDataRequest.FromString(filterString);

            var query = await context.Companies
                .DistinctColumnValuesAsync(req.Filters, columnName, pageSize, page, context);

            return query;
        }

        public static async Task DistinctTest(PostgresContext context)
        {
            var query = await context.As
                .Include(x => x.B)
                .ThenInclude(x => x.C)
                .GroupBy(x => x.B.C)
                .Select(x => x.FirstOrDefault())
                .ToListAsync();
        }

        public static async Task JoinTest(PostgresContext context)
        {
            var query = await context.As
                .Select(x => x.B.C)
                .Distinct()
                .ToListAsync();
        }

        public static async Task DirectTest(PostgresContext context)
        {
            var query = await context.As.Include(x => x.B).ThenInclude(x => x.C).ToListAsync();
        }
    }
}