using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EntityTestFramework.Exceptions;

namespace EntityTestFramework.ExpressionHelpers
{
    internal static class ExpressionAnalyser
    {
        private static string GetAssertionString(Expression expression)
        {
            var binaryExpression = expression as BinaryExpression;

            if (binaryExpression == null)
                return expression.ToString();

            var left = binaryExpression.Left as MemberExpression;

            return $"{left?.Member.Name ?? "Unknown Expression"} {binaryExpression.NodeType} \"{GetExpressionValue(binaryExpression.Right)}\"";
        }

        private static object GetExpressionValue(Expression expression)
        {
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                return constantExpression.Value;
            }

            var objectMember = Expression.Convert(expression, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }

        public static void CheckMessagesDoNotMatch<T>(List<T> messages, Expression<Func<T, bool>> predicate)
        {
            var binaryParts = ExpressionExtractor.GetBinaryChecks<T>(predicate).ToList();

            var messageList = messages.ToList();

            var errors = new List<Expression>();

            foreach (var binaryCheck in binaryParts)
            {
                messageList = messageList.Where(message => binaryCheck.Compile().Invoke(message)).ToList();

                if (messageList.Any())
                {
                    var expression = binaryCheck.Body as BinaryExpression;
                    errors.Add(expression);
                }
            }

            if (messageList.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine(typeof(T).Name + "(s) found with:");

                foreach (var err in errors)
                {
                    sb.AppendLine(GetAssertionString(err));
                }

                throw new EntitiesMatchedException(sb.ToString());
            }
        }

        public static void CheckMessagesMatch<T>(List<T> messages, Expression<Func<T, bool>> predicate)
        {
            if (!messages.Any(predicate.Compile()))
            {
                var binaryParts = ExpressionExtractor.GetBinaryChecks<T>(predicate).ToList();

                var messageList = messages.ToList();

                foreach (var binaryCheck in binaryParts)
                {
                    messageList = messageList.Where(message => binaryCheck.Compile().Invoke(message)).ToList();

                    if (!messageList.Any())
                    {
                        throw new NoEntitiesMatchedException($"No {typeof (T).Name} found where {GetAssertionString(binaryCheck.Body)}");
                    }
                }
            }
        }
    }
}