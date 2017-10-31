using System;
using System.Linq.Expressions;

namespace Intent.SoftwareFactory.Helpers
{
    public static class ExpressionParser
    {
        public static ParsedExpression<TParameter, TResult> Parse<TParameter, TResult>(string expression, string parameterName)
        {
            //const string exp = @"(Person.Age > 3 AND Person.Weight > 50) OR Person.Age < 3";
            var p = Expression.Parameter(typeof(TParameter), parameterName);
            var e = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { p }, null, expression);

            return new ParsedExpression<TParameter, TResult>(e.Compile());
        }
    }

    public class ParsedExpression<TParameter, TResult>
    {
        private readonly Delegate _compiledExpression;

        public ParsedExpression(Delegate compiledExpression)
        {
            _compiledExpression = compiledExpression;
        }

        public TResult Invoke(TParameter parameter)
        {
            return (TResult)_compiledExpression.DynamicInvoke(parameter);
        }
    }
}


