using BaseConverter;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Exceptions;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.Dtos;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Enums;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.FilterTests.SingleTypes;

[Collection("Database collection")]
public class IntTest(DatabaseFixture fixture) : ITypedTests<decimal>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Customers;

        var query = set
            .Where(x => false).ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.TotalOrders),
            ComparisonType = ComparisonType.Equal,
            Values = []
        };

        var qString = new MagicQuery([request], null);

        var result = set.FilterAndOrder(qString.ToString()).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public async Task TestEmptyValues_Paginated()
    {
        var set = _context.Customers;
        
        var query2 = set
            .Where(x => false)
            .Select(x=>new CustomerDto
            {
                Id = x.Id,
                Name = x.FirstName,
                Email = x.Email,
                Age = x.Age,
                TotalOrders = x.TotalOrders,
                SocialId = x.SocialId,
                BirthDay = x.BirthDay,
                CreatedAt = x.CreatedAt,
                CategoryId = PandaBaseConverter.Base10ToBase36(x.CategoryId),
                Categories = new List<CategoryType>()
            }).ToList();

        var paginationQuery = new PageQueryRequest
        {
            FilterQuery = new FilterQuery
            {
                PropertyName = nameof(CustomerFilter.TotalOrders),
                ComparisonType = ComparisonType.Equal,
                Values = []
            }.ToString(),
            Page = 1,
            PageSize = 10
        };
        
        var result2 = await set.FilterOrderPaginateAsync(paginationQuery, x => new CustomerDto
        {
            Id = x.Id,
            Name = x.FirstName,
            Email = x.Email,
            Age = x.Age,
            TotalOrders = x.TotalOrders,
            SocialId = x.SocialId,
            BirthDay = x.BirthDay,
            CreatedAt = x.CreatedAt,
            CategoryId = PandaBaseConverter.Base10ToBase36(x.CategoryId),
            Categories = new List<CategoryType>()
        });
        result2.Data.Should().Equal(query2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void TestNotNullable(decimal value)
    {
        var set = _context.Customers;

        var query = set
            .Where(x => x.TotalOrders == value)
            .OrderByDescending(x => x.Id)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.TotalOrders),
            ComparisonType = ComparisonType.Equal,
            Values = [value]
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
        var set = _context.Customers;

        int? data = value == "" ? null : int.Parse(value);

        var query = set
            .Where(x => x.Age == data)
            .OrderByDescending(x => x.Id)
            .ToList();

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.Age),
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
        var set = _context.Customers;

        var request = new FilterQuery
        {
            PropertyName = nameof(CustomerFilter.TotalOrders),
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