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
public class EnumTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public async Task TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.PaymentStatus as object)
            .OrderBy(x => (int)x)
            .ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.PaymentStatus)
        };

        var result = await set.ColumnDistinctValuesAsync(request);
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var filter = new FilterQuery
        {
            Values = [PaymentStatus.Pending.ToString()],
            ComparisonType = ComparisonType.Equal,
            PropertyName = nameof(OrderFilter.PaymentStatus)
        };
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.PaymentStatus),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);
        
        query.Should().Equal(result.Values);
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var filter = new FilterQuery
        {
            Values = [(int)PaymentStatus.Pending],
            ComparisonType = ComparisonType.Equal,
            PropertyName = nameof(OrderFilter.PaymentStatus)
        };
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(OrderFilter.PaymentStatus),
            FilterQuery = filter.ToString()!
        };

        var result = await set.ColumnDistinctValuesAsync(request);
        
        query.Should().Equal(result.Values);
    }
}