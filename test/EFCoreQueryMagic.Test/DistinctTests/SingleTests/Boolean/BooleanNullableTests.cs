using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.SingleTests.Boolean;

[Collection("Database collection")]
public class BooleanNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Returned as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.Returned), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Returned as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.Returned), 20, 1);

        query.Should().Equal(result.Values);
    }
}