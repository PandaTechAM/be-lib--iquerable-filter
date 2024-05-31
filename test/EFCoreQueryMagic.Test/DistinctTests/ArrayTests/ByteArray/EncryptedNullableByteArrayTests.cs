using EFCoreQueryMagic.Converters;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using EFCoreQueryMagic.Test.EntityFilters;
using EFCoreQueryMagic.Test.Infrastructure;
using FluentAssertions;
using Pandatech.Crypto;

namespace EFCoreQueryMagic.Test.DistinctTests.ArrayTests.ByteArray;

[Collection("Database collection")]
public class EncryptedNullableByteArrayTests(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;
    private readonly Aes256 _aes256 = fixture.Aes256;

    [Fact]
    public void TestDistinctColumnValuesAsync()
    {
        var set = _context.Customers;

        EncryptedConverter.Aes256 = _aes256;
        var converter = new EncryptedConverter();

        var query = set
            .Select(x => x.SocialId).ToList()
            .Select(x => converter.ConvertFrom(x) as object)
            .OrderBy(x => x)
            .Skip(0).Take(20).ToList();

        var request = new ColumnDistinctValueQueryRequest
        {
            Page = 1,
            PageSize = 20,
            ColumnName = nameof(CustomerFilter.SpecialDocumentId)
        };

        var result = set.ColumnDistinctValuesAsync(request).Result;

        query.Should().Equal(result.Values);
    }
}