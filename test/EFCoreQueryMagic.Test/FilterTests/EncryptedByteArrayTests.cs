using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.PostgresContext;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.FilterTests;

[Collection("Database collection")]
public class EncryptedByteArrayTests(DatabaseFixture fixture): ITypedTests<decimal>
{
    private readonly TestDbContext _context = fixture.Context;
    private readonly Aes256 _aes256 = fixture.Aes256;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Customers;

        var data = _aes256.Encrypt("", false).Take(64).ToArray();
        EncryptedConverter.Aes256 = _aes256;
        
        var query = set
            .Where(x => PostgresDbContext.substr(x.FirstName,1,64) == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(CustomerFilter.FirstName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("1234567890")]
    public void TestNotNullable(string value)
    {
        var set = _context.Customers;

        var data = _aes256.Encrypt(value, false).Take(64).ToArray();
        EncryptedConverter.Aes256 = _aes256;
        
        var query = set
            .Where(x => PostgresDbContext.substr(x.FirstName,1,64) == data).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value],
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(CustomerFilter.FirstName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567890")]
    public void TestNullable(string? value)
    {
        var set = _context.Customers;

        var data = _aes256.Encrypt(value, false).Take(64).ToArray();
        EncryptedConverter.Aes256 = _aes256;
        
        var query = set
            .Where(x => x.SocialId == null ? value == null :
                 PostgresDbContext.substr(x.SocialId,1,64) == data
                ).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [value], 
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(CustomerFilter.SocialId)
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
                    ComparisonType = ComparisonType.Equal,
                    PropertyName = nameof(CustomerFilter.FirstName)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters);
        Assert.Empty(result);
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