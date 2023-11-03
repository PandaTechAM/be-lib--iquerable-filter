using Microsoft.EntityFrameworkCore;

namespace PandaTech.IEnumerableFilters.PostgresContext;

public abstract class PostgresDbContext : DbContext
{
    protected PostgresDbContext(DbContextOptions options) : base(options)
    {
    }

    [DbFunction("substr", IsBuiltIn = true)]
    public static byte[] substr(byte[] target, int start, int count)
    {
        throw new Exception();
    }

    [DbFunction("rtrim", Schema = "public")]
    public static byte[] rtrim(byte[] from, byte[] remove)
    {
        throw new NotSupportedException();
    }
}