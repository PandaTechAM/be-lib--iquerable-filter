using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.Entities;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.ByteArray;

[Collection("Database collection")]
public class ByteArrayNullableTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    // todo: check when postgres db is used for tests
    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        var query0 = set.ToList();

        
        var query1 = query0.Select(x => x.SomeByteArray);
        var query2 = query1.SelectMany(x => x ?? []).Select(x => x as byte?);
        
        if (query1.Contains(null))
        {
            query2 = query2.Append(null);
        }
        
        var query3 = query2.Select(x => x as object);
        var query4 = query3.Distinct();
        var query5 = query4.OrderBy(x => x);
        var query6 = query5.Skip(0).Take(20).ToList();


        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.SomeByteArray)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query6.Should().Equal(result.Values);
    }
}