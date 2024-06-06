using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Exceptions;
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
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.LastName)
        };

        await Assert.ThrowsAsync<UnsupportedFilterException>(async () => await set.ColumnDistinctValuesAsync(request));
    }
}