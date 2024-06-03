using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Test.DistinctTests.ConverterTests;

[Collection("Database collection")]
public class BaseConverterTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Items;

        var query = set
            .Select(x => PandaBaseConverter.Base10ToBase36(x.OrderId) as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        query = query.MoveNullToTheBeginning();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(ItemFilter.OrderId)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("2")]
    public void TestDistinctColumnValuesAsync_WithValue(string? value)
    {
        var set = _context.Items;

        var query = set
            .Where(x => x.OrderId == PandaBaseConverter.Base36ToBase10(value))
            .OrderByDescending(x => x.Id)
            .Select(x => PandaBaseConverter.Base10ToBase36(x.OrderId) as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var filter = new FilterQuery
        {
            Values = [value],
            ComparisonType = ComparisonType.In,
            PropertyName = nameof(ItemFilter.OrderId)
        };

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(ItemFilter.OrderId),
            FilterQuery = filter.ToString()!
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }
}