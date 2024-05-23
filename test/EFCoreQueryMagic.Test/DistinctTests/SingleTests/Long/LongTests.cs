using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.SingleTests.Long;

[Collection("Database collection")]
public class LongTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    
    [Fact]
    public void TestDistinctColumnValuesAsyncWithPandaBaseConverter()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Id).ToList()
            .Select(x => PandaBaseConverter.Base10ToBase36(x) as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.Id), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValuesWithPandaBaseConverter()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Id).ToList()
            .Select(x => PandaBaseConverter.Base10ToBase36(x) as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Id), 20, 1);
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Quantity as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.Quantity), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Quantity as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.Quantity), 20, 1);
        
        query.Should().Equal(result.Values);
    }
}