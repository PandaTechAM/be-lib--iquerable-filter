using EFCoreQueryMagic.Dto;
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
        return;

        var set = _context.Customers;

        var query = set.ToList()
            .SelectMany(x => x.LastName)
            .Select(x => x as object)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.LastName), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    // todo: check when postgres db is used for tests
    [Fact]
    public void TestDistinctColumnValues()
    {
        return;

        var set = _context.Customers;

        var query = set.ToList()
            .SelectMany(x => x.LastName)
            .Select(x => x as object)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.LastName), 20, 1);

        query.Should().Equal(result.Values);
    }
}