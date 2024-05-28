using System.Linq.Dynamic.Core;
using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.FilterTests.SingleTypes;
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

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(ItemFilter.OrderId), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_WithValue()
    {
        var set = _context.Items;

        var value = "1";
        var query = set
            .Where(x => x.OrderId == PandaBaseConverter.Base36ToBase10(value))
            .Select(x => x.OrderId as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

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

        var test = _context.Categories
            .Include(x => x.Customers)
            .AsQueryable();

        var result = await test
            .DistinctColumnValuesAsync(qString.Filters, nameof(ItemFilter.OrderId), 20, 1, _context);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.BirthDay as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CategoryFilter.BirthDay), 20, 1);

        query.Should().Equal(result.Values);
    }
}