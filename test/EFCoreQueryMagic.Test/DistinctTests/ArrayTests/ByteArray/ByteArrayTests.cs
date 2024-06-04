using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.ByteArray;

[Collection("Database collection")]
public class ByteArrayTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    // todo: check when postgres db is used for tests
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set.ToList()
            .OrderByDescending(x => x.Id)
            .SelectMany(x => x.LastName)
            .Select(x => x as object)
            .Distinct()
            .OrderBy(x => x)
            .Skip(0).Take(20).ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.LastName)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }
}