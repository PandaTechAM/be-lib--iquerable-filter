using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.SingleTypes;

[Collection("Database collection")]
public class EnumTest(DatabaseFixture fixture): ITypedTests<int>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Orders;

        var query = set
            .Where(x => false).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.PaymentStatus),
            ComparisonType = ComparisonType.Equal,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    public void TestNotNullable(string value)
    {
        var set = _context.Orders;
        
        var data = Enum.Parse<PaymentStatus>(value);

        var query = set
            .Where(x => x.PaymentStatus == data)
            .OrderByDescending(x => x.Id)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.PaymentStatus),
            ComparisonType = ComparisonType.Equal,
            Values = [data]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("0")]
    [InlineData("1")]
    public void TestNullable(string? value)
    {
        var set = _context.Orders;

        CancellationStatus? data = value == null ? null : Enum.Parse<CancellationStatus>(value);
        
        var query = set
            .Where(x => x.CancellationStatus == data)
            .OrderByDescending(x => x.Id)
            .ToList();
        
        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.CancellationStatus),
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
        var set = _context.Orders;

        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.PaymentStatus),
            ComparisonType = ComparisonType.Equal,
            Values = [null]
        };

        var qString = new MagicQuery([request], null);
        
        Assert.Throws<UnsupportedValueException>(() => set.FilterAndOrder(qString.ToString()));
    }

    public void TestEqual(int value)
    {
        throw new NotImplementedException();
    }

    public void TestNotEqual(int value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThan(int value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThanOrEqual(int value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThan(int value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThanOrEqual(int value)
    {
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData(0)]
    public void TestContains(int value)
    {
        var set = _context.Orders;
        
        var data = (PaymentStatus)value;
        
        var request = new FilterQuery
        {
            PropertyName = nameof(OrderFilter.PaymentStatus),
            ComparisonType = ComparisonType.Contains,
            Values = [data]
        };

        var qString = new MagicQuery([request], null);

        Assert.Throws<ComparisonNotSupportedException>(() => set.FilterAndOrder(qString.ToString()));
    }

    public void TestStartsWith(int value)
    {
        throw new NotImplementedException();
    }

    public void TestEndsWith(int value)
    {
        throw new NotImplementedException();
    }

    public void TestIn(int value)
    {
        throw new NotImplementedException();
    }

    public void TestNotIn(int value)
    {
        throw new NotImplementedException();
    }

    public void TestIsNotEmpty(int value)
    {
        throw new NotImplementedException();
    }

    public void TestIsEmpty(int value)
    {
        throw new NotImplementedException();
    }

    public void TestBetween(int value)
    {
        throw new NotImplementedException();
    }

    public void TestNotContains(int value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountEqualTo(int value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountBetween(int value)
    {
        throw new NotImplementedException();
    }

    public void TestIsTrue(int value)
    {
        throw new NotImplementedException();
    }

    public void TestIsFalse(int value)
    {
        throw new NotImplementedException();
    }
}