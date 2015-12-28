namespace FakeItEasy.Expressions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using FakeItEasy.Extensibility;
    using FakeItEasy.Api;
    using System;
    using System.Diagnostics;
    using System.Text;

    /// <summary>
    /// Handles the matching of fake object calls to expressions.
    /// </summary>
    internal class ExpressionCallMatcher
    {
        private IEnumerable<IArgumentValidator> argumentValidators;
        private MethodInfoManager methodInfoManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher"/> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <param name="validatorFactory">The validator factory.</param>
        public ExpressionCallMatcher(LambdaExpression callSpecification, ArgumentValidatorFactory validatorFactory, MethodInfoManager methodInfoManager)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");
            Guard.IsNotNull(validatorFactory, "validatorFactory");
            Guard.IsNotNull(methodInfoManager, "methodInfoManager");

            this.methodInfoManager = methodInfoManager;
            this.Method = GetMethodInfo(callSpecification);

            this.argumentValidators = GetArgumentValidators(callSpecification, validatorFactory).ToArray();
        }

        public MethodInfo Method { get; private set; }

        private static MethodInfo GetMethodInfo(LambdaExpression callSpecification)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return methodExpression.Method;
            }

            var memberExpression = callSpecification.Body as MemberExpression;
            if (memberExpression != null && memberExpression.Member.MemberType == MemberTypes.Property)
            {
                var property = memberExpression.Member as PropertyInfo;
                return property.GetGetMethod();
            }

            throw new ArgumentException(ExceptionMessages.CreatingExpressionCallMatcherWithNonMethodOrPropertyExpression);
        }

        private static IEnumerable<IArgumentValidator> GetArgumentValidators(LambdaExpression callSpecification, ArgumentValidatorFactory validatorFactory)
        {
            var methodExpression = callSpecification.Body as MethodCallExpression;
            if (methodExpression != null)
            {
                return
                    (from argument in methodExpression.Arguments
                     select validatorFactory.GetArgumentValidator(argument));
            }

            return Enumerable.Empty<IArgumentValidator>();
        }

        /// <summary>
        /// Represents a factory that can create an ExpressionCallManager for the specified
        /// expression.
        /// </summary>
        /// <param name="callSpecification">The expression to create a call matcher for.</param>
        /// <returns>A call matcher for the specified expression.</returns>
        public delegate ExpressionCallMatcher Factory(LambdaExpression callSpecification);

        /// <summary>
        /// Matcheses the specified call against the expression.
        /// </summary>
        /// <param name="call">The call to match.</param>
        /// <returns>True if the call is matched by the expression.</returns>
        public virtual bool Matches(IFakeObjectCall call)
        {
            return this.InvokesSameMethodOnTarget(call.FakedObject.GetType(), call.Method, this.Method)
                && this.ArgumentsMatches(call.Arguments);
        }

        private bool InvokesSameMethodOnTarget(Type type, MethodInfo first, MethodInfo second)
        {
            return this.methodInfoManager.WillInvokeSameMethodOnTarget(type, first, second);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append(this.Method.DeclaringType.FullName);
            result.Append(".");
            result.Append(this.Method.Name);
            this.AppendArgumentsListString(result);

            return result.ToString();
        }

        private void AppendArgumentsListString(StringBuilder result)
        {
            result.Append("(");
            bool firstArgument = true;

            foreach (var validator in this.argumentValidators)
            {
                if (!firstArgument)
                {
                    result.Append(", ");
                }
                else
                {
                    firstArgument = false;
                }

                result.Append(validator.ToString());
            }

            result.Append(")");
        }

        private bool ArgumentsMatches(ArgumentCollection argumentCollection)
        {
            foreach (var argumentValidatorPair in argumentCollection.AsEnumerable().Pairwise(this.argumentValidators))
            {
                if (!argumentValidatorPair.Second.IsValid(argumentValidatorPair.First))
                {
                    return false;
                }
            }

            return true;
        }
    }
}