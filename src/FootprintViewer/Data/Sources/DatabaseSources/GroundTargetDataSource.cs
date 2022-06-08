using FootprintViewer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FootprintViewer.Data.Sources
{
    public class GroundTargetDataSource : IDataSource<GroundTargetInfo>
    {
        private readonly DbContextOptions<FootprintViewerDbContext> _options;

        public GroundTargetDataSource(DbContextOptions<FootprintViewerDbContext> options)
        {
            _options = options;
        }

        public async Task<List<GroundTargetInfo>> GetValuesAsync(IFilter<GroundTargetInfo>? filter)
        {
            var context = new FootprintViewerDbContext(_options);

            if (filter == null || filter.Names == null)
            {
                return await context.GroundTargets.Select(s => new GroundTargetInfo(s)).ToListAsync();
            }

            var list = filter.Names.ToList();

            Expression<Func<GroundTarget, bool>> predicate = s => false;

            foreach (var name in list)
                predicate = predicate.Or(s => string.Equals(s.Name, name));

            return await context.GroundTargets
                  .Where(predicate)
                  .Select(s => new GroundTargetInfo(s)).ToListAsync();
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
