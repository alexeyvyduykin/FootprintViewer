using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Managers
{
    public class GroundTargetDataManager : BaseDataManager<GroundTarget, IDatabaseSource>
    {
        protected override async Task<List<GroundTarget>> GetNativeValuesAsync(IDatabaseSource dataSource, IFilter<GroundTarget>? filter)
        {
            var options = extns2.BuildDbContextOptions<DbCustomContext>(dataSource);
            using var context = new GroundTargetDbContext(dataSource.Table, options);

            if (filter == null || filter.Names == null)
            {
                return await context.GroundTargets.ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<GroundTarget, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.GroundTargets
                  .Where(predicate).ToListAsync();
        }

        protected override async Task<List<T>> GetValuesAsync<T>(IDatabaseSource dataSource, IFilter<T>? filter, Func<GroundTarget, T> converter)
        {
            var options = extns2.BuildDbContextOptions<DbCustomContext>(dataSource);
            using var context = new GroundTargetDbContext(dataSource.Table, options);

            if (filter == null || filter.Names == null)
            {
                return await context.GroundTargets.Select(s => converter(s)).ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<GroundTarget, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.GroundTargets
                  .Where(predicate)
                  .Select(s => converter(s)).ToListAsync();
        }
    }

    public static class LINQKitExtensions
    {
        private class RebindParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParameter;
            private readonly ParameterExpression _newParameter;

            public RebindParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
            {
                _oldParameter = oldParameter;
                _newParameter = newParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == _oldParameter)
                {
                    return _newParameter;
                }

                return base.VisitParameter(node);
            }
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var expr2Body = new RebindParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2Body), expr1.Parameters);
        }
    }
}
