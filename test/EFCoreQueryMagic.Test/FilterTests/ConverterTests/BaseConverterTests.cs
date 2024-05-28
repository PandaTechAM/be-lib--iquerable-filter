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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(ItemFilter.OrderId)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(ItemFilter.OrderId)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters, _context).ToList();

        query.Should().Equal(result);
    }
}