using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PandaTech.IEnumerableFilters;
using PandaTech.Mapper;
using TestFilters.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Logging.ClearProviders();

builder.Services.AddDbContext<Context>(
    optionsBuilder =>
        optionsBuilder.UseNpgsql("Server=127.0.0.1;Database=xyz;Username=postgres;Password=example"), ServiceLifetime.Scoped
);

#region Mappers

builder.Services.AddScoped<FilterProvider>();
builder.Services.AddScoped<IMapping<Person, PersonDto>, PersonDtoMapper>();

builder.Services.AddSingleton<Counter>();

builder.Services.AddScoped<UpCounter2>();
builder.Services.AddScoped<FilterProvider>();
builder.Services.AddScoped<UpCounter>();

builder.Services.AddHttpClient();

#endregion




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