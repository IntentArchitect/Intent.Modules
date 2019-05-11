﻿using JetBrains.Annotations;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Dynamic.Core.Validation;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq.Dynamic.Core.Parser
{
    internal class ExpressionHelper : IExpressionHelper
    {
        private readonly IConstantExpressionWrapper _constantExpressionWrapper = new ConstantExpressionWrapper();
        private readonly ParsingConfig _parsingConfig;

        internal ExpressionHelper([NotNull] ParsingConfig parsingConfig)
        {
            Check.NotNull(parsingConfig, nameof(parsingConfig));

            _parsingConfig = parsingConfig;
        }

        public void WrapConstantExpression(ref Expression argument)
        {
            if (_parsingConfig.UseParameterizedNamesInDynamicQuery)
            {
                _constantExpressionWrapper.Wrap(ref argument);
            }
        }

        public void ConvertNumericTypeToBiggestCommonTypeForBinaryOperator(ref Expression left, ref Expression right)
        {
            if (left.Type == right.Type)
            {
                return;
            }

            if (left.Type == typeof(ulong) || right.Type == typeof(ulong))
            {
                right = right.Type != typeof(ulong) ? Expression.Convert(right, typeof(ulong)) : right;
                left = left.Type != typeof(ulong) ? Expression.Convert(left, typeof(ulong)) : left;
            }
            else if (left.Type == typeof(long) || right.Type == typeof(long))
            {
                right = right.Type != typeof(long) ? Expression.Convert(right, typeof(long)) : right;
                left = left.Type != typeof(long) ? Expression.Convert(left, typeof(long)) : left;
            }
            else if (left.Type == typeof(uint) || right.Type == typeof(uint))
            {
                right = right.Type != typeof(uint) ? Expression.Convert(right, typeof(uint)) : right;
                left = left.Type != typeof(uint) ? Expression.Convert(left, typeof(uint)) : left;
            }
            else if (left.Type == typeof(int) || right.Type == typeof(int))
            {
                right = right.Type != typeof(int) ? Expression.Convert(right, typeof(int)) : right;
                left = left.Type != typeof(int) ? Expression.Convert(left, typeof(int)) : left;
            }
            else if (left.Type == typeof(ushort) || right.Type == typeof(ushort))
            {
                right = right.Type != typeof(ushort) ? Expression.Convert(right, typeof(ushort)) : right;
                left = left.Type != typeof(ushort) ? Expression.Convert(left, typeof(ushort)) : left;
            }
            else if (left.Type == typeof(short) || right.Type == typeof(short))
            {
                right = right.Type != typeof(short) ? Expression.Convert(right, typeof(short)) : right;
                left = left.Type != typeof(short) ? Expression.Convert(left, typeof(short)) : left;
            }
            else if (left.Type == typeof(byte) || right.Type == typeof(byte))
            {
                right = right.Type != typeof(byte) ? Expression.Convert(right, typeof(byte)) : right;
                left = left.Type != typeof(byte) ? Expression.Convert(left, typeof(byte)) : left;
            }
        }

        public Expression GenerateAdd(Expression left, Expression right)
        {
            return Expression.Add(left, right);
        }

        public Expression GenerateStringConcat(Expression left, Expression right)
        {
            return GenerateStaticMethodCall("Concat", left, right);
        }

        public Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        public Expression GenerateEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);

            WrapConstantExpressions(ref left, ref right);

            return Expression.Equal(left, right);
        }

        public Expression GenerateNotEqual(Expression left, Expression right)
        {
            OptimizeForEqualityIfPossible(ref left, ref right);

            WrapConstantExpressions(ref left, ref right);

            return Expression.NotEqual(left, right);
        }

        public Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                var leftPart = left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left;
                var rightPart = right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right;
                return Expression.GreaterThan(leftPart, rightPart);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.GreaterThan(left, right);
        }

        public Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.GreaterThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.GreaterThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.GreaterThanOrEqual(left, right);
        }

        public Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThan(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.LessThan(left, right);
        }

        public Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type == typeof(string))
            {
                return Expression.LessThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }

            if (left.Type.GetTypeInfo().IsEnum || right.Type.GetTypeInfo().IsEnum)
            {
                return Expression.LessThanOrEqual(left.Type.GetTypeInfo().IsEnum ? Expression.Convert(left, Enum.GetUnderlyingType(left.Type)) : left,
                    right.Type.GetTypeInfo().IsEnum ? Expression.Convert(right, Enum.GetUnderlyingType(right.Type)) : right);
            }

            WrapConstantExpressions(ref left, ref right);

            return Expression.LessThanOrEqual(left, right);
        }

        public void OptimizeForEqualityIfPossible(ref Expression left, ref Expression right)
        {
            // The goal here is to provide the way to convert some types from the string form in a way that is compatible with Linq to Entities.
            // The Expression.Call(typeof(Guid).GetMethod("Parse"), right); does the job only for Linq to Object but Linq to Entities.
            Type leftType = left.Type;
            Type rightType = right.Type;

            if (rightType == typeof(string) && right.NodeType == ExpressionType.Constant)
            {
                right = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)right).Value, leftType) ?? right;
            }

            if (leftType == typeof(string) && left.NodeType == ExpressionType.Constant)
            {
                left = OptimizeStringForEqualityIfPossible((string)((ConstantExpression)left).Value, rightType) ?? left;
            }
        }

        public Expression OptimizeStringForEqualityIfPossible(string text, Type type)
        {
            if (type == typeof(DateTime) && DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return Expression.Constant(dateTime, typeof(DateTime));
            }
#if !NET35
            if (type == typeof(Guid) && Guid.TryParse(text, out Guid guid))
            {
                return Expression.Constant(guid, typeof(Guid));
            }
#else
            try
            {
                return Expression.Constant(new Guid(text));
            }
            catch
            {
                // Doing it in old fashion way when no TryParse interface was provided by .NET
            }
#endif
            return null;
        }

        private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            var methodInfo = left.Type.GetMethod(methodName, new[] { left.Type, right.Type });
            if (methodInfo == null)
            {
                methodInfo = right.Type.GetMethod(methodName, new[] { left.Type, right.Type });
            }

            return methodInfo;
        }

        private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] { left, right });
        }

        private void WrapConstantExpressions(ref Expression left, ref Expression right)
        {
            if (_parsingConfig.UseParameterizedNamesInDynamicQuery)
            {
                _constantExpressionWrapper.Wrap(ref left);
                _constantExpressionWrapper.Wrap(ref right);
            }
        }

        public Expression GenerateAndAlsoNotNullExpression(Expression sourceExpression)
        {
            var expresssions = CollectExpressions(sourceExpression);
            if (!expresssions.Any())
            {
                return null;
            }

            // Reverse the list
            expresssions.Reverse();

            // Convert all expressions into '!= null'
            var binaryExpressions = expresssions.Select(expression => Expression.NotEqual(expression, Expression.Constant(null))).ToArray();

            // Convert all binary expressions into `AndAlso(...)`
            var andAlsoExpression = binaryExpressions[0];
            for (int i = 1; i < binaryExpressions.Length; i++)
            {
                andAlsoExpression = Expression.AndAlso(andAlsoExpression, binaryExpressions[i]);
            }

            return andAlsoExpression;
        }

        private static Expression GetMemberExpression(Expression expression)
        {
            if (expression is ParameterExpression parameterExpression)
            {
                return parameterExpression;
            }

            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            if (expression is LambdaExpression lambdaExpression)
            {
                if (lambdaExpression.Body is MemberExpression bodyAsMemberExpression)
                {
                    return bodyAsMemberExpression;
                }

                if (lambdaExpression.Body is UnaryExpression bodyAsunaryExpression)
                {
                    return bodyAsunaryExpression.Operand;
                }
            }

            return null;
        }

        private static List<Expression> CollectExpressions(Expression sourceExpression)
        {
            var list = new List<Expression>();
            Expression expression = GetMemberExpression(sourceExpression);

            while (expression is MemberExpression memberExpression)
            {
                expression = GetMemberExpression(memberExpression.Expression);
                if (expression is MemberExpression)
                {
                    list.Add(expression);
                }
            }

            if (expression is ParameterExpression)
            {
                list.Add(expression);
            }

            return list;
        }
    }
}
