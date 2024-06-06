using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Test.Dtos;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.Enums;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.EntityFilters;

public class CategoryFilter
{
    [MappedToProperty(nameof(Category.Id), ConverterType = typeof(FilterPandaBaseConverter))]
    [Order]
    public int Id { get; set; }

    [MappedToProperty(nameof(Category.Categories))]
    public List<CategoryType> Categories { get; set; } = null!;

    [MappedToProperty(nameof(Category.Customers), ConverterType = typeof(BirthDayConverter))]
    public List<DateTime> BirthDay { get; set; }

    [MappedToProperty(nameof(Category.CategoryNames), ConverterType = typeof(NamesConverter))]
    public List<string> Names { get; set; } = null!;

    public List<CustomerFilter> Customers { get; set; } = null!;
}

public class BirthDayConverter : IConverter<DateTime?, Customer?>
{
    public Customer? ConvertTo(DateTime? from)
    {
        if (from is null) return null;

        var date = DateTime.SpecifyKind(from.Value, DateTimeKind.Utc);
        return Context.Set<Customer>()
            .First(x => x.BirthDay == date);
    }

    public DateTime? ConvertFrom(Customer? to)
    {
        return to?.BirthDay;
    }

    public DbContext Context { get; set; }
}

public class NamesConverter : IConverter<DistinctColumnValuesWithTranslations?, CategoryName?>
{
    public CategoryName? ConvertTo(DistinctColumnValuesWithTranslations? from)
    {
        if (from is null) return null;

        return Context.Set<CategoryName>()
            .FirstOrDefault(x => x.Id == from.Id);
    }

    public DistinctColumnValuesWithTranslations? ConvertFrom(CategoryName? to)
    {
        return to is null
            ? null
            : new DistinctColumnValuesWithTranslations
            {
                Id = to.Id,
                EnglishUs = to.NameEn,
                Russian = to.NameRu,
                Armenian = to.NameAm
            };
    }

    public DbContext Context { get; set; }
}