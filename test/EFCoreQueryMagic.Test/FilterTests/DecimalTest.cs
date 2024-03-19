using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests;

[Collection("Database collection")]
public class DecimalTest(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => false).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.TotalAmount)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }
    
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
                new FilterDto
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

    private const decimal zero = 0;
    private const decimal hundred = 100;
    
    [Theory]
    [InlineData("")]
    [InlineData("0.00")]
    [InlineData("1000.00")]
    public void TestNullable(string value)
    {
        var set = _context.Orders;

        decimal? data = null;
        data = value == "" ? null : decimal.Parse(value);
        
        var query = set
            .Where(x => x.Min == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data], 
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.Min)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);
    }
}