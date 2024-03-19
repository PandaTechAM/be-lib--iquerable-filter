using System.Runtime.InteropServices;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test;

[Collection("Database collection")]
public class DecimalTest(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(150)]
    [InlineData(250)]
    public void TestNotNullable(decimal value)
    {
        
        var set = _context.Orders;
        
        var query = set
            .Where(x => x.TotalAmount > value).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto()
                {
                    Values = [value], 
                    ComparisonType = ComparisonType.GreaterThan,
                    PropertyName = nameof(OrderFilter.TotalAmount)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);

    }
    
    

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(100)]
    public void TestNullable(decimal? value)
    {
        var set = _context.Orders;
        
        var query = set
            .Where(x => x.TotalAmount > value).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto()
                {
                    Values = [value], 
                    ComparisonType = ComparisonType.GreaterThan,
                    PropertyName = nameof(OrderFilter.TotalAmount)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);
    }
}