using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Demo;
using EFCoreQueryMagic.Demo.Components;
using EFCoreQueryMagic.Demo.db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pandatech.Crypto;

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

/*var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<PostgresContext>();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
await db.Populate(1000);
await db.PopulateTest();*/

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/api/generate/{count:int}", (PostgresContext context, int count) => context.Populate(count));
app.MapGet("/api/companies", (PostgresContext context, [FromQuery] int page, [FromQuery] int pageSize,
    [FromQuery] string q) => SemiController.Companies(context, page, pageSize, q));
app.MapGet("/api/companies/distinct/{columnName}", (PostgresContext context, [FromRoute] string columnName,
        [FromQuery] string filterString, [FromQuery] int page, [FromQuery] int pageSize) =>
    SemiController.DistinctColumnValues(context, columnName, filterString, page, pageSize));

app.Run();
