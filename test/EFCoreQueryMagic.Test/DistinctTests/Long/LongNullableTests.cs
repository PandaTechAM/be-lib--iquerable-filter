using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.Long;

[Collection("Database collection")]
public class LongNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsyncWithPandaBaseConverter()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.OrderId).ToList()
            .Select(x => PandaBaseConverter.Base10ToBase36(x) as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.OrderId), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesWithPandaBaseConverter()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.OrderId).ToList()
            .Select(x => PandaBaseConverter.Base10ToBase36(x) as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.OrderId), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.VerifiedQuantity as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.VerifiedQuantity), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.VerifiedQuantity as object)
            .Distinct().OrderByDescending(x => x).ThenBy(x => x)
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.VerifiedQuantity), 20, 1);

        query.Should().Equal(result.Values);
    }
}