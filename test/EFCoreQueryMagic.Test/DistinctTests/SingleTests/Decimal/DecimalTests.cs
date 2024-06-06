using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.SingleTests.Decimal;

[Collection("Database collection")]
public class DecimalTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.TotalAmount as object)
            .Distinct()
            .OrderBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.TotalAmount)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }
}