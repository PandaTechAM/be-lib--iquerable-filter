using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.FilterTests.SingleTypes;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.FilterTests.ConverterTests;

[Collection("Database collection")]
public class ListDtoTest(DatabaseFixture fixture) : ITypedTests<string>
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void TestEmptyValues()
    {
        var set = _context.Categories;

        var query = set
            .Where(x => false).ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CategoryFilter.BirthDay)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters).ToList();

        query.Should().Equal(result);
    }

    [Theory]
    [InlineData("2024-03-10 00:00:00.000")]
    [InlineData("2024-05-10 00:00:00.000")]
    public void TestNotNullable(DateTime value)
    {
        var set = _context.Categories;

        var date = value.ToUniversalTime();

        var query = set
            .Where(x => x.Customers.Any(y => y.BirthDay == date))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [date],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CategoryFilter.BirthDay)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters, _context).ToList();

        query.Should().Equal(result);
    }

    // OK
    [Theory]
    [InlineData(null)]
    [InlineData("2024-03-10 00:00:00.000")]
    public void TestNullable(string? value)
    {
        var set = _context.Categories;

        _ = DateTime.TryParse(value, out var date);
        date = date.ToUniversalTime();
        
        var query = set
            .Where(x => x.Customers.Any(y => y.BirthDay == date))
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        var qString = new GetDataRequest
        {
            Filters =
            [
                new FilterDto
                {
                    Values = [date],
                    ComparisonType = ComparisonType.Contains,
                    PropertyName = nameof(CategoryFilter.BirthDay)
                }
            ]
        };

        var result = set.ApplyFilters(qString.Filters, _context).ToList();

        query.Should().Equal(result);
    }

    [Fact]
    public void TestNotNullableWithNullableValue()
    {
        throw new NotImplementedException();
    }

    public void TestEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThan(string value)
    {
        throw new NotImplementedException();
    }

    public void TestGreaterThanOrEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThan(string value)
    {
        throw new NotImplementedException();
    }

    public void TestLessThanOrEqual(string value)
    {
        throw new NotImplementedException();
    }

    public void TestContains(string value)
    {
        throw new NotImplementedException();
    }

    public void TestStartsWith(string value)
    {
        throw new NotImplementedException();
    }

    public void TestEndsWith(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIn(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotIn(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsNotEmpty(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsEmpty(string value)
    {
        throw new NotImplementedException();
    }

    public void TestBetween(string value)
    {
        throw new NotImplementedException();
    }

    public void TestNotContains(string value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountEqualTo(string value)
    {
        throw new NotImplementedException();
    }

    public void TestHasCountBetween(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsTrue(string value)
    {
        throw new NotImplementedException();
    }

    public void TestIsFalse(string value)
    {
        throw new NotImplementedException();
    }
}