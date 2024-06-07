using System.Linq.Dynamic.Core;
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
public class EnumArrayNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .ToList()
            .SelectMany(x => x.Statuses ?? [])
            .Select(x => x as object)
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Statuses)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .ToList()
            .SelectMany(x => x.Statuses)
            .Select(x => x as object)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Skip(0).Take(20)
            .ToList();
        
        var filter = new FilterQuery
        {
            Values = [CustomerStatus.Active.ToString()],
            ComparisonType = ComparisonType.In,
            PropertyName = nameof(CustomerFilter.Statuses)
        };

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Statuses),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.Statuses.Contains(CustomerStatus.Active))
            .ToList()
            .SelectMany(x => x.Statuses)
            .Select(x => x as object)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Skip(0).Take(20)
            .ToList();

        var filter = new FilterQuery
        {
            Values = [(int)CustomerStatus.Active],
            ComparisonType = ComparisonType.In,
            PropertyName = nameof(CustomerFilter.Statuses)
        };

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Statuses),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query);
    }
}