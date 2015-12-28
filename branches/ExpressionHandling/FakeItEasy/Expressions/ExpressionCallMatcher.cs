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
    public class ExpressionCallMatcher
    {
        private MethodInfo method;
        private IEnumerable<IArgumentValidator> argumentValidators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionCallMatcher"/> class.
        /// </summary>
        /// <param name="callSpecification">The call specification.</param>
        /// <param name="validatorFactory">The validator factory.</param>
        public ExpressionCallMatcher(LambdaExpression callSpecification, ArgumentValidatorFactory validatorFactory)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");
            Guard.IsNotNull(validatorFactory, "validatorFactory");

            this.method = GetMethodInfo(callSpecification);

            this.argumentValidators = GetArgumentValidators(callSpecification, validatorFactory).ToArray();
        }

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
            //return Helpers.IsSameMethodOrDerivative(call.FakedObject.GetType(), call.Method, this.method) && this.ArgumentsMatches(call.Arguments);
            //return this.IsCallToExpressionMethod(call.Method) && this.ArgumentsMatches(call.Arguments);
            return WillInvokeSameMethodWhenCalledOnTarget(call.FakedObject, call.Method, this.method)
                && this.ArgumentsMatches(call.Arguments);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            result.Append(this.method.DeclaringType.FullName);
            result.Append(".");
            result.Append(this.method.Name);
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

        public static bool WillInvokeSameMethodWhenCalledOnTarget(object target, MethodInfo firstMethod, MethodInfo secondMethod)
        {
            if (IsSameMethod(firstMethod, secondMethod))
            {
                return true;
            }

            var methodInvokedByFirst = GetMethodOnTypeThatWillBeInvokedByMethodInfo(target.GetType(), firstMethod);
            var methodInvokedBySecond = GetMethodOnTypeThatWillBeInvokedByMethodInfo(target.GetType(), secondMethod);

            return methodInvokedByFirst != null && methodInvokedBySecond != null && IsSameMethod(methodInvokedByFirst, methodInvokedBySecond);
        }

        [DebuggerStepThrough]
        private static bool IsSameMethod(MethodInfo first, MethodInfo second)
        {
            return first.GetBaseDefinition().Equals(second.GetBaseDefinition());
        }

        private struct TypeMethodInfoPair
        {
            public Type Type;
            public MethodInfo MethodInfo;

            public override int GetHashCode()
            {
                return Type.GetHashCode() ^ MethodInfo.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is TypeMethodInfoPair))
                {
                    return false;
                }

                var other = (TypeMethodInfoPair)obj;

                return this.Type.Equals(other.Type) && this.MethodInfo.Equals(other.MethodInfo);
            }
        }

        private static Dictionary<TypeMethodInfoPair, MethodInfo> methodCache = new Dictionary<TypeMethodInfoPair, MethodInfo>();

        private static MethodInfo GetMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            MethodInfo result = null;
            var key = new TypeMethodInfoPair { Type = type, MethodInfo = method };

            if (!methodCache.TryGetValue(key, out result))
            {
                result = FindMethodOnTypeThatWillBeInvokedByMethodInfo(type, method);
                methodCache.Add(key, result);
            }

            return result;
        }

        private static MethodInfo FindMethodOnTypeThatWillBeInvokedByMethodInfo(Type type, MethodInfo method)
        {
            MethodInfo result = 
                (from typeMethod in type.GetMethods()
                 where IsSameMethod(typeMethod, method)
                 select typeMethod).FirstOrDefault();

            if (result != null)
            {
                return result;
            }

            result = GetMethodOnTypeThatImplementsInterfaceMethod(type, method);

            if (result != null)
            {
                return result;
            }

            return GetMethodOnInterfaceTypeImplementedByMethod(type, method);
        }

        private static MethodInfo GetMethodOnInterfaceTypeImplementedByMethod(Type type, MethodInfo method)
        {

            var allInterfaces =
                from i in type.GetInterfaces()
                where TypeImplementsInterface(method.ReflectedType, i)
                select i;

            foreach (var interfaceType in allInterfaces)
            {
                var interfaceMap = method.ReflectedType.GetInterfaceMap(interfaceType);

                var foundMethod = 
                    (from methodTargetPair in interfaceMap.InterfaceMethods.Pairwise(interfaceMap.TargetMethods)
                     where IsSameMethod(method, methodTargetPair.Second)
                     select methodTargetPair.First).FirstOrDefault();

                if (foundMethod != null)
                {
                    return GetMethodOnTypeThatImplementsInterfaceMethod(type, foundMethod);
                }
            }

            return null;
        }

        private static MethodInfo GetMethodOnTypeThatImplementsInterfaceMethod(Type type, MethodInfo method)
        {
            var baseDefinition = method.GetBaseDefinition();

            if (!baseDefinition.DeclaringType.IsInterface || !TypeImplementsInterface(type, baseDefinition.DeclaringType))
            {
                return null;
            }

            var interfaceMap = type.GetInterfaceMap(baseDefinition.DeclaringType);
           
            return
                (from methodTargetPair in interfaceMap.InterfaceMethods.Pairwise(interfaceMap.TargetMethods)
                 where IsSameMethod(methodTargetPair.First, method)
                 select methodTargetPair.Second).First();
        }

        private static bool TypeImplementsInterface(Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(x => x.Equals(interfaceType));
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