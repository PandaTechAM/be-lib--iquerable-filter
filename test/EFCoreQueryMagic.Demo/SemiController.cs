using EFCoreQueryMagic.Demo.db;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using EFCoreQueryMagic.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Demo;

static class SemiController
{
    public static async Task<PagedResponse<Company>> Companies(db.PostgresContext context, [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string q)
    {
        
        var pqr = new PageQueryRequest()
        {
            Page = page,
            PageSize = pageSize,
            FilterQuery = q
        };

        return await context.Companies
            .Include(x => x.OneToManys)
            .Include(x => x.SomeClass)
            .FilterOrderPaginateAsync(pqr);
    }

    public static async Task<ColumnDistinctValues> DistinctColumnValues(db.PostgresContext context,
        [FromQuery] string columnName,
        [FromQuery] string filterString, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var req = new ColumnDistinctValueQueryRequest()
        {
            Page = page, PageSize = pageSize, ColumnName = columnName, FilterQuery = filterString
        };

        var query = await context.Companies
            .ColumnDistinctValuesAsync(req);

        return query;
    }

    public static async Task DistinctTest(db.PostgresContext context)
    {
        var query = await context.As
            .Include(x => x.B)
            .ThenInclude(x => x.C)
            .GroupBy(x => x.B.C)
            .Select(x => x.FirstOrDefault())
            .ToListAsync();
    }

    public static async Task JoinTest(db.PostgresContext context)
    {
        var query = await context.As
            .Select(x => x.B.C)
            .Distinct()
            .ToListAsync();
    }

    public static async Task DirectTest(db.PostgresContext context)
    {
        var query = await context.As.Include(x => x.B).ThenInclude(x => x.C).ToListAsync();
    }
}