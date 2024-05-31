using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EFCoreQueryMagic.Extensions;

internal static class DbContextExtensions
{
    internal static DbContext GetDbContext<T>(this IQueryable<T> query)
    {
        if (query is IInfrastructure<IServiceProvider> infrastructure)
        {
            //var infrastructure = (IInfrastructure<IServiceProvider>)query;
            var serviceProvider = infrastructure.Instance;
            var currentContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
            return currentContext!.Context;
        }
        
        throw new InvalidOperationException("The query is not associated with a DbContext.");
    }
}