using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test;

[Collection("Database collection")]
public class UnitTest1(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void Test1()
    {
        
        var set = _context.Orders;
        
        var query = set
            .Where(x => x.TotalAmount > 150).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto()
                {
                    Values = [150], 
                    ComparisonType = ComparisonType.GreaterThan,
                    PropertyName = nameof(OrderFilter.TotalAmount)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);

    }
}