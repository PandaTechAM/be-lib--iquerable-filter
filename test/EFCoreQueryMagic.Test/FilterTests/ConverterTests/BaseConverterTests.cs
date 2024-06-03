using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.ConverterTests;

[Collection("Database collection")]
public class BaseConverterTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Items;

        var query = set
            .Where(x => false).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.OrderId),
            ComparisonType = ComparisonType.In,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("1")]
    public void TestNullable(string? value)
    {
        var set = _context.Items;

        var query = set
            .Where(x => x.OrderId == PandaBaseConverter.Base36ToBase10(value))
            .Distinct()
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.OrderId),
            ComparisonType = ComparisonType.In,
            Values = [value]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Select(x=>x.Id).OrderBy(x=>x)
            .Should()
            .Equal(result.Select(x=>x.Id).OrderBy(x=>x));
    }
}