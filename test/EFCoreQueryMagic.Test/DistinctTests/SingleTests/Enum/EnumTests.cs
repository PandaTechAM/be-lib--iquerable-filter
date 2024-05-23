using EFCoreQueryMagic.Dto;
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
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.PaymentStatus as object)
            .OrderBy(x => (int)x)
            .ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_String()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [PaymentStatus.Pending.ToString()],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_Number()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)PaymentStatus.Pending],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1);

        query.Should().Equal(result.Values);
    }


    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Orders;

        var query = set
            .Select(x => x.PaymentStatus as object)
            .OrderBy(x => (int)x)
            .ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_String()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [PaymentStatus.Pending.ToString()],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_Number()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => x.PaymentStatus == PaymentStatus.Pending)
            .Select(x => x.PaymentStatus as object)
            .ToList();

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)PaymentStatus.Pending],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(OrderFilter.PaymentStatus), 20, 1);

        query.Should().Equal(result.Values);
    }
}