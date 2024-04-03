using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.Boolean;

[Collection("Database collection")]
public class BooleanTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Paid as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.Paid), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.Paid as object)
            .Distinct().OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.Paid), 20, 1);
        
        query.Should().Equal(result.Values);
    }
}