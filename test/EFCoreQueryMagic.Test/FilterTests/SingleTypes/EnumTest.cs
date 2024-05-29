using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

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
            .Where(x => x.PaymentStatus == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data], 
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
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
            .Where(x => x.CancellationStatus == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data], 
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.CancellationStatus)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);
    }
    
    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Orders;

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [null],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        };

        Assert.Throws<UnsupportedValueException>(() => set.ApplyFilters(qString.Filters));
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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [data], 
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(OrderFilter.PaymentStatus)
                }
            ]
        };

        Assert.Throws<ComparisonNotSupportedException>(() => set.ApplyFilters(qString.Filters).ToList());
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