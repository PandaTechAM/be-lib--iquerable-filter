using System.Text.Json.Serialization;

namespace TestFilters.Controllers.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Sex
{
    Male,
    Female
}