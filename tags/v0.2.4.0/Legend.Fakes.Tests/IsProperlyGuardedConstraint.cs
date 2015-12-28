using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.Configuration;
using Legend.Fakes.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Legend.Fakes.Tests
{
    public class IsProperlyGuardedConstraint
            : Constraint
    {

        public static void IsProperlyGuarded(Expression<Action> method)
        {
            Assert.That(method, new IsProperlyGuardedConstraint());
        }

        public static void IsProperlyGuarded<T>(Expression<Func<T>> method)
        {
            Assert.That(method, new IsProperlyGuardedConstraint());
        }

        private Action<MessageWriter> descriptionWriter;

        public override bool Matches(object actual)
        {
            this.actual = actual;
            var lambda = actual as LambdaExpression;

            if (lambda != null)
            {
                return this.Matches(lambda);
            }
            else
            {
                return false;
            }
        }

        private bool Matches(LambdaExpression expression)
        {
            var callExpression = expression.Body as MethodCallExpression;

            if (callExpression != null)
            {
                return this.ValidateCallExpression(callExpression);
            }

            var constructorExpression = expression.Body as NewExpression;

            if (constructorExpression != null)
            {
                return this.ValidateConstructorExpression(constructorExpression);
            }

            this.descriptionWriter = x => x.WriteLine("A method call expression.");
            return false;
        }

        private bool ValidateConstructorExpression(NewExpression constructorExpression)
        {
            var callsThatDidntThrow = CallAllAndReturnCallsThatDidntThrow(constructorExpression);

            if (callsThatDidntThrow.Count > 0)
            {
                descriptionWriter = x =>
                {
                    x.WriteLine("The following calls should've thrown ArgumentNullException:");
                    foreach (var call in callsThatDidntThrow)
                    {
                        WriteCallToMessageWriter(call, x);
                    }
                };
                return false;
            }

            return true;
        }

        private bool ValidateCallExpression(MethodCallExpression callExpression)
        {
            var callsThatDidntThrow = CallAllAndReturnCallsThatDidntThrow(callExpression);

            if (callsThatDidntThrow.Count > 0)
            {
                descriptionWriter = x =>
                {
                    x.WriteLine("The following calls should've thrown ArgumentNullException:");
                    foreach (var call in callsThatDidntThrow)
                    {
                        WriteCallToMessageWriter(call, x);
                    }
                };
                return false;
            }

            return true;
        }

        private static List<object[]> CallAllAndReturnCallsThatDidntThrow(MethodCallExpression callExpression)
        {
            var argumentTypes = callExpression.Method.GetParameters().Select(x => x.ParameterType).ToArray();
            IEnumerable<object[]> calls = CalculateArgumentPermutations(callExpression.Arguments, argumentTypes);

            List<object[]> callsThatDidntThrow = new List<object[]>();
            foreach (var call in calls)
            {
                bool thrown = false;
                try
                {
                    callExpression.Method.Invoke(
                        GetExpressionValue(callExpression.Object),
                        call);
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException && ((TargetInvocationException)ex).InnerException is ArgumentNullException)
                    {
                        thrown = true;
                    }
                }

                if (!thrown) callsThatDidntThrow.Add(call);
            }

            return callsThatDidntThrow;
        }

        private static List<object[]> CallAllAndReturnCallsThatDidntThrow(NewExpression callExpression)
        {
            var argumentTypes = callExpression.Constructor.GetParameters().Select(x => x.ParameterType).ToArray();
            IEnumerable<object[]> calls = CalculateArgumentPermutations(callExpression.Arguments, argumentTypes);

            List<object[]> callsThatDidntThrow = new List<object[]>();
            foreach (var call in calls)
            {
                bool thrown = false;
                try
                {
                    callExpression.Constructor.Invoke(
                        call);
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException && ((TargetInvocationException)ex).InnerException is ArgumentNullException)
                    {
                        thrown = true;
                    }
                }

                if (!thrown) callsThatDidntThrow.Add(call);
            }

            return callsThatDidntThrow;
        }

        private void WriteCallToMessageWriter(object[] call, MessageWriter writer)
        {
            var result = new StringBuilder();

            result.Append("(");

            foreach (var argument in call)
            {
                if (result.Length > 1)
                {
                    result.Append(", ");
                }

                result.Append(argument != null ? argument.ToString() : "<NULL>");
            }

            result.Append(")");

            writer.WriteLine(result.ToString());
        }

        private static IEnumerable<object[]> CalculateArgumentPermutations(IEnumerable<Expression> arguments, Type[] argumentTypes)
        {
            var result = new List<object[]>();

            int index = 0;
            foreach (var argument in arguments)
            {
                if (!argumentTypes[index].IsValueType)
                {
                    result.Add(GetArgumentPermutationWithArgumentAsNull(index, arguments));
                }
                index++;
            }

            return result;
        }

        private static object[] GetArgumentPermutationWithArgumentAsNull(int argumentIndexOfNullValue, IEnumerable<Expression> arguments)
        {
            return arguments
                .Take(argumentIndexOfNullValue).Select(x => GetExpressionValue(x))
                .Concat(new object[] { null })
                .Concat(arguments.Skip(argumentIndexOfNullValue + 1).Select(x => GetExpressionValue(x))).ToArray();
        }

        private static object GetExpressionValue(Expression expression)
        {
            if (expression == null) return null;

            var lambda = expression as LambdaExpression;

            if (lambda == null)
            {
                lambda = Expression.Lambda(expression);
            }

            return lambda.Compile().DynamicInvoke();
        }

        public override void WriteDescriptionTo(MessageWriter writer)
        {
            this.descriptionWriter(writer);
        }
    }
}
