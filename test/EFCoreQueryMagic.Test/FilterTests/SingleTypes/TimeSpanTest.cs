using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.SingleTypes;

[Collection("Database collection")]
public class TimeSpanTest(DatabaseFixture fixture) : ITypedTests<decimal>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Items;

        var query = set
            .Where(x => false).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(ItemFilter.AvailablePeriod)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(10)]
    public void TestNotNullable(long value)
    {
        var set = _context.Items;

        var data = TimeSpan.FromHours(value);

        var query = set
            .Where(x => x.AvailablePeriod == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(ItemFilter.AvailablePeriod)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("10")]
    public void TestNullable(string value)
    {
        var set = _context.Items;

        TimeSpan? data = value == "" ? null : TimeSpan.FromHours(int.Parse(value));

        var query = set
            .Where(x => x.UnavailablePeriod == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(ItemFilter.UnavailablePeriod)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Items;

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [null],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(ItemFilter.AvailablePeriod)
                }
            ]
        };

        Assert.Throws<UnsupportedValueException>(() => set.ApplyFilters(qString.Filters));
    }

    public void TestEqual(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestNotEqual(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThan(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThanOrEqual(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThan(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThanOrEqual(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestContains(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestStartsWith(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestEndsWith(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestIn(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestNotIn(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestIsNotEmpty(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestIsEmpty(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestBetween(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestNotContains(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountEqualTo(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountBetween(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestIsTrue(decimal value)
    {
        throw new NotImplementedException();
    }

    public void TestIsFalse(decimal value)
    {
        throw new NotImplementedException();
    }
}