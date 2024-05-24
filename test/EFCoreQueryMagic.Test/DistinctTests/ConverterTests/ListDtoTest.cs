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
public class ListDtoTest(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.BirthDay as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CategoryFilter.BirthDay), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_WithValue()
    {
        var set = _context.Customers;

        var value = Convert.ToDateTime("2024-03-10 00:00:00.000").ToUniversalTime();
        var query = set
            .Where(x => x.BirthDay == value)
            .Select(x => x.BirthDay as object)
            .Distinct()
            .Skip(0).Take(20).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CategoryFilter.BirthDay)
                }
            ]
        };

        var test = _context.Categories
            .Include(x => x.Customers);
        
        var result = await test
            .DistinctColumnValuesAsync(qString.Filters, nameof(CategoryFilter.BirthDay), 20, 1, _context);

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