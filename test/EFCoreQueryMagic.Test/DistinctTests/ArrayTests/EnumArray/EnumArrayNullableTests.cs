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
        
        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.Statuses)
        };

        var result = await set.ColumnDistinctValuesAsync(request);

        result.Values.Should().Equal(Enum.GetValues(typeof(CustomerStatus)).ToDynamicList());
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Customers;

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

        result.Values.Should().Equal(Enum.GetValues(typeof(CustomerStatus)).ToDynamicList());
    }

    [Fact]
    public async Task TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Customers;

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

        result.Values.Should().Equal(Enum.GetValues(typeof(CustomerStatus)).ToDynamicList());
    }
}