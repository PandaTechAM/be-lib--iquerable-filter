using EFCoreQueryMagic.Dto;
using EFCoreQueryMagic.Dto.Public;
using Microsoft.EntityFrameworkCore;

namespace EFCoreQueryMagic.Extensions;

public static class PublicExtensions
{
    public static async Task<PagedResponse<TEntity>> FilterOrderPaginateAsync<TEntity>(this IQueryable<TEntity> query,
        PageQueryRequest pageQueryRequest, CancellationToken cancellationToken = default) where TEntity : class
    {
        var pagedData = await query
            .FilterAndOrder(pageQueryRequest.FilterQuery)
            .Skip(pageQueryRequest.PageSize * (pageQueryRequest.Page - 1))
            .Take(pageQueryRequest.PageSize)
            .ToListAsync(cancellationToken: cancellationToken);

        var totalCount = await query
            //.AsNoTracking()
            .LongCountAsync(cancellationToken: cancellationToken);

        return new PagedResponse<TEntity>(pagedData, pageQueryRequest.Page, pageQueryRequest.PageSize, totalCount);
    }

    public static IQueryable<TEntity> FilterAndOrder<TEntity>(this IQueryable<TEntity> query, string filterQuery)
        where TEntity : class
    {
        if (filterQuery == "{}")
        {
            return query/*.AsNoTracking()*/;
        }

        var filter = MagicQuery.FromString(filterQuery);

        var context = query.GetDbContext();
        
        return query
            //.AsNoTracking()
            .ApplyFilters(filter.Filters, context)
            .ApplyOrdering(filter.Order);
    }

    public static Task<ColumnDistinctValues> ColumnDistinctValuesAsync<TEntity>(
        this IQueryable<TEntity> query, ColumnDistinctValueQueryRequest request,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var magicQuery = MagicQuery.FromString(request.FilterQuery);

        var context = query.GetDbContext();
        
        return query
            .AsNoTracking()
            .DistinctColumnValuesAsync(null, magicQuery, request.ColumnName, request.PageSize,
                request.Page, context, cancellationToken);
    }

    public static Task<object?> AggregateAsync<TModel>(this IQueryable<TModel> query,
        AggregateQueryRequest request,
        CancellationToken cancellationToken = default)
        where TModel : class
    {
        var filteredAndOrderedQuery = query.FilterAndOrder(request.FilterQuery);

        return filteredAndOrderedQuery
            .AggregateAsync(
                new AggregateQuery(request.ColumnName, request.AggregateType),
                cancellationToken: cancellationToken);
    }
}