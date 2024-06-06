using System.Linq.Expressions;
using EFCoreQueryMagic.Attributes;
using EFCoreQueryMagic.PostgresContext;

namespace EFCoreQueryMagic.Helpers;

internal static class EncryptedHelper
{
    public static Expression<Func<TModel, bool>> GetExpression<TModel>(MappedToPropertyAttribute attribute,
        byte[]? value)
    {
        if (value is null)
        {
            var parameter = Expression.Parameter(typeof(TModel));
            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);
            var equality = Expression.Equal(accessor, Expression.Constant(null));
            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }

        if (value.Length == 0)
        {
            var parameter = Expression.Parameter(typeof(TModel));
            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);
            var equality = Expression.Equal(accessor, Expression.Constant(Array.Empty<byte>()));
            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }
        else
        {
            var parameter = Expression.Parameter(typeof(TModel));

            var accessor = PropertyHelper.GetPropertyExpression(parameter, attribute);

            var postgresFunc =
                typeof(PostgresDbContext).GetMethod("substr", [typeof(byte[]), typeof(int), typeof(int)])!;
            
            var propertyAccess =
                Expression.Call(postgresFunc, accessor, Expression.Constant(1), Expression.Constant(64));

            var constant = Expression.Constant(value.Take(64).ToArray());

            var equality = Expression.Equal(propertyAccess, constant);

            return Expression.Lambda<Func<TModel, bool>>(equality, parameter);
        }
    }
}