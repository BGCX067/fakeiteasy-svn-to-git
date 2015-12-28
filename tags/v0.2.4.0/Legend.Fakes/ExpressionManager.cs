using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.Core.Interceptor;
using System.Reflection;

namespace Legend.Fakes
{
    internal static class ExpressionManager
    {
        public static object GetValueProducedByExpression(Expression expression)
        {
            var lambda = Expression.Lambda(expression).Compile();
            return lambda.DynamicInvoke();
        }
    }
}
