using System.Text;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.FilterTests;

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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CustomerFilter.LastName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CustomerFilter.LastName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

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

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value], 
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CustomerFilter.MiddleName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();
        
        query.Should().Equal(result);
    }
    
    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        var set = _context.Customers;

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [null],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CustomerFilter.LastName)
                }
            ]
        };

        Assert.Throws<UnsupportedValueException>(() => set.ApplyFilters(qString.Filters));
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