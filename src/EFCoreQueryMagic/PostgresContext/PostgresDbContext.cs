using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.PostgresContext;

public abstract class PostgresDbContext : DbContext
{
    protected PostgresDbContext(DbContextOptions options) : base(options)
    {
    }

    [DbFunction("substr", IsBuiltIn = true)]
    public static byte[] substr(byte[] target, int start, int count)
    {
        return target is null ? [] :  target.Take(count).ToArray();
    }

    /*
    [DbFunction("rtrim", Schema = "public")]
    public static byte[] rtrim(byte[] from, byte[] remove)
    {
        throw new NotSupportedException();
    }*/
}