using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EFCoreQueryMagic.Extensions;

internal static class DbContextExtensions
{
    internal static DbContext GetDbContext<T>(this IQueryable<T> query)
    {
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var queryCompiler = typeof(EntityQueryProvider).GetField("_queryCompiler", bindingFlags)!.GetValue(query.Provider);
        var queryContextFactory = queryCompiler!.GetType().GetField("_queryContextFactory", bindingFlags)!.GetValue(queryCompiler);

        var dependencies = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", bindingFlags)!.GetValue(queryContextFactory);
        var queryContextDependencies = typeof(DbContext).Assembly.GetType(typeof(QueryContextDependencies).FullName!);
        var stateManagerProperty = queryContextDependencies!.GetProperty("StateManager", bindingFlags | BindingFlags.Public)!.GetValue(dependencies);
        var stateManager = (IStateManager)stateManagerProperty!;

        return stateManager.Context; 
        
        
        if (query is not IInfrastructure<IServiceProvider> infrastructure)
        {
            throw new InvalidOperationException("The query is not associated with a DbContext. It is possible that .AsNoTracking is used on given query");
        }

        var serviceProvider = infrastructure.Instance;
        var currentContext = serviceProvider.GetService(typeof(ICurrentDbContext)) as ICurrentDbContext;
        return currentContext!.Context;
    }
}