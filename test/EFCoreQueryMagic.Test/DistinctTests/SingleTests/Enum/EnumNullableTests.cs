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
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.CancellationStatus as object)
            .OrderBy(x => (int)x)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_String()
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

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_Number()
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

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }


    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.CancellationStatus as object)
            .OrderBy(x => (int)x)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesWithConverter()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => (int)x.CancellationStatus as object)
            .OrderBy(x => (int)x)
            .Select(x=>x.ToString() as object)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.CancellationStatus2)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;
        
        query.Should().Equal(result.Values);
    }
}