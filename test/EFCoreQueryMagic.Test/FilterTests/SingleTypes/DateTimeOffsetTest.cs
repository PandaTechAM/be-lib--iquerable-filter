using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.SingleTypes;

[Collection("Database collection")]
public class DateTimeOffsetTest(DatabaseFixture fixture) : ITypedTests<decimal>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Items;

        var query = set
            .Where(x => false).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.DateTimeOffset),
            ComparisonType = ComparisonType.Equal,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void TestNotNullable(long value)
    {
        var set = _context.Items;

        var data = new DateTimeOffset(value, TimeSpan.Zero);

        var query = set
            .Where(x => x.DateTimeOffset == data)
            .OrderByDescending(x => x.Id)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.DateTimeOffset),
            ComparisonType = ComparisonType.Equal,
            Values = [data]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("10")]
    public void TestNullable(string value)
    {
        var set = _context.Items;

        DateTimeOffset? data = value == "" ? null : new DateTimeOffset(Convert.ToInt32(value), TimeSpan.Zero);

        var query = set
            .Where(x => x.DateTimeOffsetNullable == data)
            .OrderByDescending(x => x.Id)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.DateTimeOffsetNullable),
            ComparisonType = ComparisonType.Equal,
            Values = [data]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Items;
        
        var request = new FilterQuery
        {
            PropertyName = nameof(ItemFilter.DateTimeOffset),
            ComparisonType = ComparisonType.Equal,
            Values = [null]
        };

        var qString = new MagicQuery([request], null);

        Assert.Throws<UnsupportedValueException>(() => set.FilterAndOrder(qString.ToString()));
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