using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Enums;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.FilterTests.SingleTypes;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;


namespace EFCoreQueryMagic.Test.FilterTests.OtherTests;

[Collection("Database collection")]
public class TestTracking(DatabaseFixture fixture)
{
    [Fact]
    public void TestSelect()
    {
        var set = fixture.Context.Customers;

        var query = set.Select(x => x.Category).ToList();

        var qString = "{}";

        var result = set
            .FilterAndOrder(qString, x => x.Category)
            .ToList();

        query.Should().Equal(result);
    }
}

