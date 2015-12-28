using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Api;
using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.TestHelpers
{
    public static class ExpressionHelper
    {
        public static ExpressionCallRule CreateRule<TFake, TReturn>(Expression<Func<TFake, TReturn>> expression)
        {
            return GetCallRuleFactory().Invoke(expression);
        }

        public static ExpressionCallRule CreateRule<TFake>(Expression<Action<TFake>> expression)
        {
            return GetCallRuleFactory().Invoke(expression);
        }

        public static ExpressionCallMatcher CreateMatcher<TFake, TReturn>(Expression<Func<TFake, TReturn>> expression)
        {
            return ServiceLocator.Current.Resolve<ExpressionCallMatcher.Factory>().Invoke(expression);
        }

        public static ExpressionCallMatcher CreateMatcher<TFake>(Expression<Action<TFake>> expression)
        {
            return ServiceLocator.Current.Resolve<ExpressionCallMatcher.Factory>().Invoke(expression);
        }

        public static IFakeObjectCall CreateFakeCall<TFake, Treturn>(Expression<Func<TFake, Treturn>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), (LambdaExpression)callSpecification);
        }

        public static IFakeObjectCall CreateFakeCall<TFake>(Expression<Action<TFake>> callSpecification)
        {
            return CreateFakeCall(A.Fake<TFake>(), (LambdaExpression)callSpecification);
        }

        public static Expression GetArgumentExpression<T>(Expression<Action<T>> callSpecification, int argumentIndex)
        {
            return GetArgumentExpression((LambdaExpression)callSpecification, argumentIndex);
        }

        public static Expression GetArgumentExpression<T, TReturn>(Expression<Func<T, TReturn>> callSpecification, int argumentIndex)
        {
            return GetArgumentExpression((LambdaExpression)callSpecification, argumentIndex);
        }

        private static ExpressionCallRule.Factory GetCallRuleFactory()
        {
            return ServiceLocator.Current.Resolve<ExpressionCallRule.Factory>();
        }

        private static Expression GetArgumentExpression(LambdaExpression callSpecification, int argumentIndex)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            return methodExpression.Arguments[argumentIndex];
        }

        private static IFakeObjectCall CreateFakeCall<TFake>(TFake fakedObject, LambdaExpression callSpecification)
        {
            var result = A.Fake<IFakeObjectCall>();

            Fake.Configure(result).CallsTo(x => x.Method).Returns(GetMethodInfo(callSpecification));
            Fake.Configure(result).CallsTo(x => x.FakedObject).Returns(fakedObject);
            Fake.Configure(result).CallsTo(x => x.Arguments).Returns(CreateArgumentCollection(fakedObject, callSpecification));

            return result;
        }

        private static MethodInfo GetMethodInfo(LambdaExpression callSpecification)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return methodExpression.Method;
            }

            var memberExpression = callSpecification.Body as MemberExpression;
            var property = memberExpression.Member as PropertyInfo;
            return property.GetGetMethod();
        }


        private static MethodCallExpression GetMethodExpression(LambdaExpression callSpecification)
        {
            return callSpecification.Body as MethodCallExpression;
        }

        private static ArgumentCollection CreateArgumentCollection<TFake>(TFake fake, LambdaExpression callSpecification)
        {
            var methodCall = callSpecification.Body as MethodCallExpression;
            
            MethodInfo method = null;
            object[] arguments = null;

            if (methodCall != null)
            {
                method = methodCall.Method;
                arguments =
                    (from argument in methodCall.Arguments
                     select ExpressionManager.GetValueProducedByExpression(argument)).ToArray();
            }
            else
            {
                var propertyCall = callSpecification.Body as MemberExpression;
                var property = propertyCall.Member as PropertyInfo;

                method = property.GetGetMethod();
                arguments = new object[] { };
            }
            
            return new ArgumentCollection(arguments, method);

        }
    }
}
