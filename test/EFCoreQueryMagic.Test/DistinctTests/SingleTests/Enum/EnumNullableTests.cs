using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.SingleTests.Enum;

[Collection("Database collection")]
public class EnumNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.CancellationStatus as object)
            .Distinct()
            .OrderBy(x => (int)x == null ? 0 : 1)
            .ThenBy(x => x)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query!);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.CancellationStatus == CancellationStatus.Yes)
            .Select(x => x.CancellationStatus as object)
            .ToList();

        var filter = new FilterQuery
        {
            Values = [CancellationStatus.Yes.ToString()],
            ComparisonType = ComparisonType.Equal,
            PropertyName = nameof(OrderFilter.CancellationStatus)
        };

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query!);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.CancellationStatus == CancellationStatus.Yes)
            .Select(x => x.CancellationStatus as object)
            .ToList();

        var filter = new FilterQuery
        {
            Values = [(int)CancellationStatus.Yes],
            ComparisonType = ComparisonType.Equal,
            PropertyName = nameof(OrderFilter.CancellationStatus)
        };

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query!);
    }

    [Fact]
    public async Task TestDistinctColumnValuesWithConverter()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => (int)x.CancellationStatus)
            .Distinct()
            .OrderBy(x => x == null ? 0 : 1)
            .ThenBy(x => x)
            .Select(x => x.ToString() as object)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus2)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(query!);
    }
}