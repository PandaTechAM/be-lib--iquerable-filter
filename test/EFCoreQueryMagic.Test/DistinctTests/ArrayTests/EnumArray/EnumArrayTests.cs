using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.EnumArray;

[Collection("Database collection")]
public class EnumArrayTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Types).AsEnumerable()
            .SelectMany(x => x)
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Types)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Types.Contains(CustomerType.Seller))
            .Select(x => x.Types)
            .AsEnumerable()
            .SelectMany(x=>x)
            .Select(x => x as object).ToList()
            .Distinct()
            .ToList();

        var filter = new FilterQuery
        {
            Values = [CustomerType.Seller.ToString()],
            ComparisonType = ComparisonType.In,
            PropertyName = nameof(CustomerFilter.Types)
        };
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Types),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Types.Contains(CustomerType.Seller))
            .Select(x => x.Types)
            .AsEnumerable()
            .SelectMany(x=>x)
            .Select(x => x as object).ToList()
            .Distinct()
            .ToList();

        var filter = new FilterQuery
        {
            Values = [(int)CustomerType.Seller],
            ComparisonType = ComparisonType.In,
            PropertyName = nameof(CustomerFilter.Types)
        };
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Types),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        query.Should().Equal(result.Values);
    }
}