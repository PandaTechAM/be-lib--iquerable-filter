using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.FilterTests.SingleTypes;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.FilterTests.Arrays;

[Collection("Database collection")]
public class ByteArrayTests(DatabaseFixture fixture): ITypedTests<byte>
{
    private readonly TestDbContext _context = fixture.Context;
    private readonly Aes256 _aes256 = fixture.Aes256;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.LastName.Contains((byte)'\0')).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.LastName),
            ComparisonType = ComparisonType.Contains,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void TestNotNullable(byte value)
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.LastName.Contains(value)).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.LastName),
            ComparisonType = ComparisonType.Contains,
            Values = [value]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData("0")]
    [InlineData("5")]
    public void TestNullable(string? value)
    {
        var set = _context.Customers;

        byte? data = value == null ? null : byte.Parse(value);
        
        var query = set
            .Where(x => 
                x.MiddleName == null
                    ? value == null
                    : data == null || x.MiddleName.Contains(data.Value)
                ).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.MiddleName),
            ComparisonType = ComparisonType.Contains,
            Values = [data]
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();
        
        query.Should().Equal(result);
    }
    
    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Customers;

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.LastName),
            ComparisonType = ComparisonType.Contains,
            Values = [null]
        };

        var qString = new MagicQuery([request], null);

        Assert.Throws<UnsupportedValueException>(() => set.FilterAndOrder(qString.ToString()));
    }

    public void TestEqual(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestNotEqual(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThan(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThanOrEqual(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThan(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThanOrEqual(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestContains(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestStartsWith(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestEndsWith(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestIn(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestNotIn(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestIsNotEmpty(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestIsEmpty(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestBetween(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestNotContains(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountEqualTo(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountBetween(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestIsTrue(byte value)
    {
        throw new NotImplementedException();
    }

    public void TestIsFalse(byte value)
    {
        throw new NotImplementedException();
    }
}