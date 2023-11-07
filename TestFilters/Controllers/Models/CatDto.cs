using PandaTech.IEnumerableFilters.Attributes;
using PandaTech.IEnumerableFilters.Converters;

namespace TestFilters.Controllers.Models;

[MappedToClass(typeof(Cat))]
public class CatDto
{   
    [PandaPropertyBaseConverter]
    [MappedToProperty(nameof(Cat.Id),
        ConverterType = typeof(FilterPandaBaseConverter))]
    public long Id { get; set; }

    [MappedToProperty(nameof(Cat.Name))]
    public string Name { get; set; } = null!;

    [MappedToProperty(nameof(Cat.Age))]
    public int Age { get; set; }

    [MappedToProperty(nameof(Cat.SomeBytes), Encrypted = true)]
    public string EncryptedString { get; set; } = null!;

    [MappedToProperty(nameof(Cat.Types), SubPropertyRoute = nameof(CatTypes.Name))]
    public string CatType { get; set; } = null!;
}