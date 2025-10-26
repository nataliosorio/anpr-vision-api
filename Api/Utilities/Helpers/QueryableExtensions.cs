using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, IDictionary<string, string?> filters)
        {
            if (filters == null || !filters.Any())
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? finalExpr = null;

            foreach (var filter in filters)
            {
                var propertyInfo = typeof(T).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null) continue;

                var property = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(Convert.ChangeType(filter.Value, propertyInfo.PropertyType));

                var equalExpr = Expression.Equal(property, constant);

                finalExpr = finalExpr == null
                    ? equalExpr
                    : Expression.AndAlso(finalExpr, equalExpr);
            }

            if (finalExpr == null)
                return query;

            var lambda = Expression.Lambda<Func<T, bool>>(finalExpr, parameter);
            return query.Where(lambda);
        }
    }
}
