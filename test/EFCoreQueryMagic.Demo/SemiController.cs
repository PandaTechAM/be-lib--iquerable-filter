using EFCoreQueryMagic.Demo.db;
using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Demo;

static class SemiController
{
    public static async Task<List<Company>> Companies(db.PostgresContext context, [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string q)
    {
        var req = GetDataRequest.FromString(q);

        return await context.Companies
            .ApplyFilters(req.Filters, context)
            .Include(x => x.OneToManys)
            .Include(x => x.SomeClass)
            .ApplyOrdering(req.Order)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public static async Task<DistinctColumnValues> DistinctColumnValues(db.PostgresContext context,
        [FromQuery] string columnName,
        [FromQuery] string filterString, [FromQuery] int page, [FromQuery] int pageSize)
    {
        var req = GetDataRequest.FromString(filterString);

        var query = await context.Companies
            .DistinctColumnValuesAsync(req.Filters, columnName, pageSize, page, context);

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