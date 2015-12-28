using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FakeItEasy.Extensibility;
using System.Reflection;

namespace FakeItEasy
{
    public static class Argument
    {
        public static IExtensibleIs Is 
        {
            get
            {
                return null;
            }
        }

        internal static bool IsCallWithArgumentValidator(Expression expression)
        {
            var methodExpression = expression as MethodCallExpression;

            if (methodExpression == null)
            {
                return false;
            }

            var attribute = GetArgumentValidatorAttribute(methodExpression);
                
            return attribute != null;
        }

        private static ArgumentValidatorAttribute GetArgumentValidatorAttribute(MethodCallExpression expression)
        {
            return (from a in expression.Method.GetCustomAttributes(false).Cast<Attribute>()
                    where a.GetType().Equals(typeof(ArgumentValidatorAttribute))
                    select a).Cast<ArgumentValidatorAttribute>().FirstOrDefault();
        }

        internal static IArgumentValidator GetArgumentValidatorForArgument(MethodCallExpression expression)
        {
            var argumentsForConstructor = expression.Arguments.Skip(1).Select(x => ExpressionManager.GetValueProducedByExpression(x)).ToArray();
            var validatorType = GetValidatorType(expression);

            AssertValidatorTypeImplementsArgumentValidatorInterface(validatorType);

            EnsureThatValidatorConstructorMatchesSignatureOfExtensionMethod(expression.Method, validatorType);

            var validator = (IArgumentValidator)Activator.CreateInstance(validatorType, argumentsForConstructor);
            

            return validator;
        }

        private static void AssertValidatorTypeImplementsArgumentValidatorInterface(Type validatorType)
        {
            if (!typeof(IArgumentValidator).IsAssignableFrom(validatorType))
            {
                throw new ArgumentValidationException(ExceptionMessages.NonArgumentValidatorTypeExceptionMessage.FormatInvariant(validatorType.FullName));
            }
        }

        private static void EnsureThatValidatorConstructorMatchesSignatureOfExtensionMethod(MethodInfo validationExtensionMethod, Type validatorType)
        {
            var extensionMethodArgumentTypes = validationExtensionMethod.GetParameters().Skip(1).Select(x => x.ParameterType);
            var constructor = validatorType.GetConstructor(extensionMethodArgumentTypes.ToArray());

            if (constructor == null)
            {
                throw new ArgumentValidationException(
                    ExceptionMessages.ArgumentValidatorConstructorSignatureDoesntMatchValidationMethod.FormatInvariant(
                        validatorType.FullName, CreateSignatureString(extensionMethodArgumentTypes)));
            }
        }

        private static string CreateSignatureString(IEnumerable<Type> types)
        {
            var result = new StringBuilder();
            result.Append("[");

            foreach (var type in types)
            {
                if (result.Length > 1)
                {
                    result.Append(", ");
                }

                result.Append(type.FullName);
            }

            result.Append("]");

            return result.ToString();
        }

        private static Type GetValidatorType(MethodCallExpression expression)
        {
            var validatorType = GetArgumentValidatorAttribute(expression).ValidatorType;

            if (validatorType.IsGenericTypeDefinition)
            {
                var typeArgumentsForExtensionMethod = expression.Method.GetGenericArguments();

                EnsureThatTypeArgumentsOfArgumentValidatorMatchesThoseOfTheExtensionMethod(typeArgumentsForExtensionMethod, validatorType);

                validatorType = validatorType.MakeGenericType(typeArgumentsForExtensionMethod);
            }

            return validatorType;
        }

        private static void EnsureThatTypeArgumentsOfArgumentValidatorMatchesThoseOfTheExtensionMethod(Type[] typeArgumentsForExtensionMethod, Type validatorType)
        {
            if (typeArgumentsForExtensionMethod.Length != validatorType.GetGenericArguments().Length)
            {
                throw new ArgumentValidationException(ExceptionMessages.ArgumentValidatorTypeArgumentsDoesntMatchValidationMethod
                    .FormatInvariant(typeArgumentsForExtensionMethod.Length, validatorType.GetGenericArguments().Length));
            }
        }
    }
}