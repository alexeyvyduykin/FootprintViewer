using System;
using System.Linq.Expressions;

namespace FootprintViewer.Data.Managers
{
    //public static class LINQKitExtensions
    //{
    //    //private class RebindParameterVisitor : ExpressionVisitor
    //    //{
    //    //    private readonly ParameterExpression _oldParameter;
    //    //    private readonly ParameterExpression _newParameter;

    //    //    public RebindParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
    //    //    {
    //    //        _oldParameter = oldParameter;
    //    //        _newParameter = newParameter;
    //    //    }

    //    //    protected override Expression VisitParameter(ParameterExpression node)
    //    //    {
    //    //        if (node == _oldParameter)
    //    //        {
    //    //            return _newParameter;
    //    //        }

    //    //        return base.VisitParameter(node);
    //    //    }
    //    //}

    //    //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    //    //{
    //    //    var expr2Body = new RebindParameterVisitor(expr2.Parameters[0], expr1.Parameters[0]).Visit(expr2.Body);
    //    //    return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2Body), expr1.Parameters);
    //    //}
    //}
}
