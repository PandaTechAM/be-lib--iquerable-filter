using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ConverterTests;

[Collection("Database collection")]
public class MappingConverterTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.ItemTypeMappings;

        var query = set
            .Select(x => x.ItemType.NameAm as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        query = query.MoveNullToTheBeginning();
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(ItemFilter.ItemTypes)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }
}