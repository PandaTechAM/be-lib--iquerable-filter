using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.DistinctTests.ConverterTests;

[Collection("Database collection")]
public class MappingConverterTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Items;

        var query = set
            .SelectMany(x => x.ItemTypeMappings)
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .ToList()
            .Select(x => new TypeConverter().ConvertFrom(x))
            .Distinct()
            .Select(x => x as object)
            .Skip(0).Take(20).ToList();
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(ItemFilter.ItemTypes)
        };

        var context = _context.Items.GetDbContext();
        
        var q1 = _context.Items.Include(x => x.ItemTypeMappings);
        var q2 = q1.ThenInclude(x => x.ItemType);
        
        var result = q2.ColumnDistinctValuesAsync(request, context).Result;

        result.Values.Should().Equal(query);
    }
}