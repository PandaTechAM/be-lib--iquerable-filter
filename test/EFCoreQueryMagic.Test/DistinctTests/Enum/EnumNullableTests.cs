using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.Enum;

[Collection("Database collection")]
public class EnumNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = System.Enum.GetValues<CancellationStatus>()
            .Select(x => x as object)
            .OrderBy(x => x)
            .ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.CancellationStatus), 20, 1).Result;
        
        query.Should().Equal(result.Values);
    }
    
    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = System.Enum.GetValues<CancellationStatus>()
            .Select(x => x as object)
            .OrderBy(x => x)
            .ToList();
        
        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.CancellationStatus), 20, 1);
        
        query.Should().Equal(result.Values);
    }
}