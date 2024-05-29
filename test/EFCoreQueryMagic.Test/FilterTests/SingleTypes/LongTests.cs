using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.SingleTypes;

[Collection("Database collection")]
public class LongTests(DatabaseFixture fixture)
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
                    PropertyName = nameof(OrderFilter.Id)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public void TestBaseConverterWithInvalidCharacter()
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
                    Values = ["ա1"],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.Id)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters);

        query.Should().Equal(result);
    }
    
    [Fact]
    public void TestBaseConverterWithValidCharacter()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.Id == PandaBaseConverter.Base36ToBase10("a1")).ToList();
        
        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = ["a1"],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.Id)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters);
        
        query.Should().Equal(result);
    }
}