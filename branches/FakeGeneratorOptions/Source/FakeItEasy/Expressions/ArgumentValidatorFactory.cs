namespace FakeItEasy.Expressions
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using System.Linq;
using System.Reflection;

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

        private static bool TryGetArgumentValidator(Expression argument, out IArgumentValidator result)
        {
            if (TryGetAbstractValidator(argument, out result))
            {
                return true;
            }

            result = null;
            return false;
        }

        private static bool TryGetAbstractValidator(Expression argument, out IArgumentValidator result)
        {
            var unary = argument as UnaryExpression;
            if (unary != null && IsArgumentValidatorConversionMethod(unary.Method))
            {
                result = ExpressionManager.GetValueProducedByExpression(unary.Operand) as IArgumentValidator;
                return true;
            }

            var member = argument as MemberExpression;
            if (member != null && IsArgumentValidatorArgumentProperty(member))
            {
                result = ExpressionManager.GetValueProducedByExpression(member.Expression) as IArgumentValidator;
                return true;
            }

            result = ExpressionManager.GetValueProducedByExpression(argument) as IArgumentValidator;
            return result != null;
        }

        private static bool IsArgumentValidatorArgumentProperty(MemberExpression member)
        {
            return
                member.Member.Name == "Argument"
                && member.Member.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentValidator<>));
        }

        private static bool IsArgumentValidatorConversionMethod(MethodInfo method)
        {
            return
                method != null
                && method.Name.Equals("op_Implicit")
                && method.DeclaringType.GetGenericTypeDefinition().Equals(typeof(ArgumentValidator<>));
        }
    }
}