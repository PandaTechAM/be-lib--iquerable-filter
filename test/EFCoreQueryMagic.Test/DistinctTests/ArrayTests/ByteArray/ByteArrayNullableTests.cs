using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.ByteArray;

[Collection("Database collection")]
public class ByteArrayNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    // todo: check when postgres db is used for tests
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set.ToList()
            .SelectMany(x => x.MiddleName)
            .Select(x => x as object)
            .Skip(0).Take(20).ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.MiddleName)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }
}