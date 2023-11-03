using System.Text.Json.Serialization;
using PandaTech.IEnumerableFilters.Attributes;

namespace TestFilters.Controllers.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MyEnum
{
    One,

    [HideEnumValue]
    Two,
    Three,
    Four
}