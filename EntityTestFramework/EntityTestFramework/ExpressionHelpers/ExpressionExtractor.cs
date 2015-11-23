using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityTestFramework.ExpressionHelpers
{
    internal static class ExpressionExtractor
    {
        private static readonly BinaryExpressionExctractor Exctractor;

        static ExpressionExtractor()
        {
            Exctractor = new BinaryExpressionExctractor();
        }

        public static IEnumerable<Expression<Func<T, bool>>> GetBinaryChecks<T>(Expression node)
        {
            Exctractor.Clear();
            Exctractor.Visit(node);

            return Exctractor.Expressions.Select(expression => Expression.Lambda<Func<T, bool>>(expression, Exctractor.ParameterExpression));
        }

        private class BinaryExpressionExctractor : ExpressionVisitor
        {
            public List<BinaryExpression> Expressions { get; }
            public ParameterExpression ParameterExpression { get; private set; }

            public BinaryExpressionExctractor()
            {
                Expressions = new List<BinaryExpression>();
            }

            public void Clear()
            {
                ParameterExpression = null;
                Expressions.Clear();
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                ParameterExpression = node;
                return base.VisitParameter(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if ( node.NodeType == ExpressionType.AndAlso
                    || node.NodeType == ExpressionType.OrElse)
                {
                    var left = Visit(node.Left);
                    var right = Visit(node.Right);
                    var conversion = Visit(node.Conversion);

                    if (left != node.Left || right != node.Right || conversion != node.Conversion)
                    {
                        if (node.NodeType == ExpressionType.Coalesce && node.Conversion != null)
                            return Expression.Coalesce(left, right, conversion as LambdaExpression);

                        return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, node.Method);
                    }
                }
                else
                {
                    Expressions.Add(node);
                }

                return node;
            }
        }
    }
}