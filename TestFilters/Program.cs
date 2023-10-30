using Microsoft.EntityFrameworkCore;
using PandaTech.IEnumerableFilters;
using PandaTech.Mapper;
using TestFilters.Controllers;
using TestFilters.Controllers.bulk;
using TestFilters.Controllers.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Logging.ClearProviders();

builder.Services.AddDbContext<Context>(
    optionsBuilder =>
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=xyz;Username=postgres;Password=example")
    // .UseSnakeCaseNamingConvention()
    , ServiceLifetime.Scoped
);

#region Mappers

builder.Services.AddScoped<IMapping<Person, PersonDto>, PersonDtoMapper>();

#endregion

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policyBuilder =>
{
    policyBuilder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();