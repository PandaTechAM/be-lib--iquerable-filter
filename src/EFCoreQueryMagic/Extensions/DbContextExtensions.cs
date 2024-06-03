using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCoreQueryMagic.Extensions;

internal static class DbContextExtensions
{
    internal static DbContext GetDbContext<T>(this IQueryable<T> query)
    {
        if (query is not IInfrastructure<IServiceProvider> infrastructure)
        {
            throw new InvalidOperationException("The query is not associated with a DbContext. It is possible that .AsNoTracking is used on given query");
        }

        var serviceProvider = infrastructure.Instance;
        var currentContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
        return currentContext!.Context;
    }
}