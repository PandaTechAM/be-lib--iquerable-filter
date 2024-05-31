using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
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

        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.Id),
            ComparisonType = ComparisonType.Equal,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public void TestBaseConverterWithInvalidCharacter()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => false).ToList();
        
        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.Id),
            ComparisonType = ComparisonType.Equal,
            Values = ["ա1"]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }
    
    [Fact]
    public void TestBaseConverterWithValidCharacter()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.Id == PandaBaseConverter.Base36ToBase10("a1")).ToList();
        
        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.Id),
            ComparisonType = ComparisonType.Equal,
            Values = ["a1"]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
}