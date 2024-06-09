using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ConverterTests;

[Collection("Database collection")]
public class ListDtoTest(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.BirthDay as object)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Skip(0).Take(20).ToList();
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CategoryFilter.BirthDay)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_WithValue()
    {
        var set = _context.Customers;

        var value = Convert.ToDateTime("2024-03-10 00:00:00.000").ToUniversalTime();
        var query = set
            .Where(x => x.BirthDay == value)
            .Select(x => x.BirthDay as object)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Skip(0).Take(20).ToList();

        var test = _context.Categories;
        
        var filter = new FilterQuery
        {
            Values = [value],
            ComparisonType = ComparisonType.Contains,
            PropertyName = nameof(CategoryFilter.BirthDay)
        };
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CategoryFilter.BirthDay),
            FilterQuery = filter.ToString()!
        };

        var result = test.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }
}