using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Cassandra.Data.Linq;
using EnsureThat;
using Uptime.Monitoring.Model.Models;

namespace Uptime.Monitoring.Cassandra.Extensions {

    public static class CqlQueryExtensions {
        
        public static async Task<IEnumerable<T>> ExecutePagedAsync<T>(this CqlQuery<T> query, Pagination pagination) {
            var resultQuery = EnsureArg.IsNotNull(query, nameof(query));

            if (pagination != null && pagination.PageSize != 0) {
                resultQuery = query.SetPageSize(pagination.PageSize);

                if (pagination.State != null && pagination.State.Length != 0) {
                    resultQuery.SetPagingState(pagination.State);
                }

                var result = await resultQuery.ExecutePagedAsync();
                pagination.State = result.CurrentPagingState;

                return result;
            }
            else {
                return await resultQuery.ExecuteAsync();
            }
        }

        public static CqlQuery<TEntity> WhereInOrEmpty<TEntity, TFieldType>(this CqlQuery<TEntity> query, Expression<Func<TEntity, TFieldType>> selector, IEnumerable<TFieldType> collection)
        {
            if (collection == null || !collection.Any())
            {
                return query;
            }

            EnsureArg.IsNotNull(query, nameof(query));
            EnsureArg.IsNotNull(selector, nameof(selector));

            var containsMethod = GetMethodInfo<IEnumerable<TFieldType>>(x => x.Contains(default));

            var selectorBody = selector.Body;
            var selectorParameter = selector.Parameters[0];

            var expression = Expression.Lambda<Func<TEntity, bool>>(
                Expression.Call(containsMethod, Expression.Constant(collection.ToList()), selectorBody),
                selectorParameter
            );

            return query.Where(expression);
        }

        private static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", nameof(expression));
        }
    }

}