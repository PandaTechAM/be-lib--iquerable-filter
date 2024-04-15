using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.EnumArray;

[Collection("Database collection")]
public class EnumArrayTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Types).AsEnumerable()
            .SelectMany(x => x)
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValuesAsync(qString.Filters, nameof(CustomerFilter.Types), 20, 1).Result;

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_String()
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

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [CustomerType.Seller.ToString()],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Types)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Types), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValuesAsync_Number()
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

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)CustomerType.Seller],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Types)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Types), 20, 1);

        query.Should().Equal(result.Values);
    }


    [Fact]
    public void TestDistinctColumnValues()
    {
        var set = _context.Customers;

        var query = set
            .Select(x => x.Types).AsEnumerable()
            .SelectMany(x => x)
            .Select(x => x as object).ToList()
            .OrderBy(x => (int)x)
            .Distinct()
            .ToList();

        var qString = new GetDataRequest();

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Types), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_String()
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

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [CustomerType.Seller.ToString()],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Types)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Types), 20, 1);

        query.Should().Equal(result.Values);
    }

    [Fact]
    public void TestDistinctColumnValues_Number()
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

        var qString = GetDataRequest.FromString(new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [(int)CustomerType.Seller],
                    ComparisonType = ComparisonType.In,
                    PropertyName = nameof(CustomerFilter.Types)
                }
            ]
        }.ToString());

        var result = set.DistinctColumnValues(qString.Filters, nameof(CustomerFilter.Types), 20, 1);

        query.Should().Equal(result.Values);
    }
}