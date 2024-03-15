using EFCoreQueryMagic.Test.Infrastructure;

namespace EFCoreQueryMagic.Test;

[Collection("Database collection")]
public class UnitTest1(DatabaseFixture fixture)
{
    private readonly TestDbContext _context = fixture.Context;

    [Fact]
    public void Test1()
    {
    }
}