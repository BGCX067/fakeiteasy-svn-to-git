namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Extensibility;
    using System.Linq;

    /// <summary>
    /// Responsible for creating argument validators from arguments in an expression.
    /// </summary>
    public class ArgumentValidatorFactory
    {
        /// <summary>
        /// Gets an argument validator for the argument represented by the expression.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <returns>An IArgumentValidator used to validated arguments in IFakeObjectCalls.</returns>
        public virtual IArgumentValidator GetArgumentValidator(Expression argument)
        {
            IArgumentValidator result = null;
            
            if (!TryGetArgumentValidator(argument, out result))
            {
                result = new EqualityArgumentValidator(ExpressionManager.GetValueProducedByExpression(argument));
            }

            return result;
        }

        private bool TryGetArgumentValidator(Expression argument, out IArgumentValidator result)
        {
            var methodExpression = GetMethodCallExpression(argument);

            if (methodExpression == null)
            {
                result = null;
                return false;
            }

            var validatorType = GetArgumentValidatorType(methodExpression);
                
            if (validatorType == null)
            {
                result = null;
                return false;
            }

            result = CreateArgumentValidatorOfType(validatorType, methodExpression);
            return true;

        }

        private static MethodCallExpression GetMethodCallExpression(Expression argument)
        {
            var methodExpression = argument as MethodCallExpression;

            if (methodExpression != null)
            {
                return methodExpression;
            }

            var conversionExpression = argument as UnaryExpression;

            if (conversionExpression != null)
            {
                methodExpression = conversionExpression.Operand as MethodCallExpression;
            }

            return methodExpression;
        }

        private static IArgumentValidator CreateArgumentValidatorOfType(Type validatorType, MethodCallExpression methodExpression)
        {
            var argumentsForConstructor = GetArgumentsForConstructor(methodExpression);

            return (IArgumentValidator)Activator.CreateInstance(validatorType, argumentsForConstructor);
        }

        private static object[] GetArgumentsForConstructor(MethodCallExpression methodExpression)
        {
            return 
                (from argumentAndType in methodExpression.Arguments.Pairwise(methodExpression.Method.GetParameters())
                 select new { Value = ExpressionManager.GetValueProducedByExpression(argumentAndType.First), Type = argumentAndType.Second.ParameterType })
                 .SkipWhile(x => x.Type.Equals(typeof(IExtensibleIs)))
                 .Select(x => x.Value)
                 .ToArray();
        }

        private static Type GetArgumentValidatorType(MethodCallExpression methodExpression)
        {
            var attribute = GetArgumentValidatorAttribute(methodExpression);

            if (attribute == null)
            {
                return null;
            }

            return EnsureThatValidatorTypeIsNotGenericTypeDefinition(attribute.ValidatorType, methodExpression);
        }

        private static Type EnsureThatValidatorTypeIsNotGenericTypeDefinition(Type validatorType, MethodCallExpression methodExpression)
        {
            var result = validatorType;
            
            if (validatorType.IsGenericTypeDefinition)
            {
                var genericParameters =
                    from g in methodExpression.Method.GetGenericArguments()
                    select g;

                result = validatorType.MakeGenericType(genericParameters.ToArray());
            }

            return result;
        }

        private static ArgumentValidatorAttribute GetArgumentValidatorAttribute(MethodCallExpression methodExpression)
        {
            return 
                (from attribute in methodExpression.Method.GetCustomAttributes(typeof(ArgumentValidatorAttribute), false).Cast<ArgumentValidatorAttribute>()
                 select attribute).SingleOrDefault();
        }

        private class EqualityArgumentValidator
            : IArgumentValidator
        {
            private object expectedValue;

            public EqualityArgumentValidator(object expectedValue)
            {
                this.expectedValue = expectedValue;
            }

            public bool IsValid(object argument)
            {
                return object.Equals(this.expectedValue, argument);
            }
        }
    }
}
