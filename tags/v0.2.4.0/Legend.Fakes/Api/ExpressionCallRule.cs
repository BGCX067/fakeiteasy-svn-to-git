using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Legend.Fakes.Extensibility;
using System.Reflection;
using Legend.Fakes.Api;
using System.Text;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.Api
{
    public class ExpressionCallRule
        : BuildableCallRule
    {
        #region Fields
        private RuleState state;
        #endregion

        #region Construction
        public ExpressionCallRule(LambdaExpression callSpecification)
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            this.Expression = callSpecification.Body;

            this.state = CreateRuleState(callSpecification);
        } 
        #endregion

        #region Properties
        public Expression Expression { get; private set; }
        #endregion

        #region Methods
        private static RuleState CreateRuleState(LambdaExpression callSpecification)
        {
            if (callSpecification.Body.NodeType == ExpressionType.Call)
            {
                return new MethodRuleState(callSpecification.Body as MethodCallExpression);
            }

            if (callSpecification.Body.NodeType == ExpressionType.MemberAccess)
            {
                return new PropertyRuleState(callSpecification.Body as MemberExpression);
            }

            throw new ArgumentException(ExceptionMessages.MemberAccessorNotCorrectExpressionType, "callSpecification");
        }

        private static IArgumentValidator GetArgumentValidator(Expression expressionArgument)
        {
            IArgumentValidator validator = null;

            if (!TryGetCustomArgumentValidator(expressionArgument, out validator))
            {
                return new EqualityArgumentValidator() { ValidArgument = ExpressionManager.GetValueProducedByExpression(expressionArgument) };
            }

            return validator;
        }

        private static bool TryGetCustomArgumentValidator(Expression expression, out IArgumentValidator validator)
        {
            if (Argument.IsCallWithArgumentValidator(expression))
            {
                validator = Argument.GetArgumentValidatorForArgument((MethodCallExpression)expression);
                return true;
            }

            validator = null;
            return false;
        }

        protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return this.state.IsApplicableTo(fakeObjectCall);
        }

        public override string ToString()
        {
            return this.state.ToString();
        }
        #endregion

        #region Nested types
        private class EqualityArgumentValidator
            : IArgumentValidator
        {
            public object ValidArgument;

            public bool IsValid(object argument)
            {
                return object.Equals(ValidArgument, argument);
            }

            public override string ToString()
            {
                return this.ValidArgument == null ? "<NULL>" : this.ValidArgument.ToString();
            }
        }

        private abstract class RuleState
        {
            public abstract bool IsApplicableTo(IFakeObjectCall fakeObjectCall);
        }

        private class MethodRuleState
            : RuleState
        {
            private MethodInfo method;
            private IEnumerable<IArgumentValidator> argumentValidators;


            public MethodRuleState(MethodCallExpression expression)
                : this(expression.Method, CreateArgumentValidators(expression))
            {
                
            }

            public MethodRuleState(MethodInfo method, IEnumerable<IArgumentValidator> argumentValidators)
            {
                this.method = method;
                this.argumentValidators = argumentValidators;
            }

            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                if (Helpers.IsSameMethodOrDerivative(fakeObjectCall.FakedObject.GetType(), this.method, fakeObjectCall.Method))
                {
                    return ArgumentsMatches(fakeObjectCall);
                }

                return false;
            }

            private static IEnumerable<IArgumentValidator> CreateArgumentValidators(MethodCallExpression expression)
            {
                return
                    (from argument in expression.Arguments
                     select GetArgumentValidator(argument)).ToArray();
            }

            private bool ArgumentsMatches(IFakeObjectCall fakeObjectCall)
            {
                using (var callArguments = fakeObjectCall.Arguments.GetEnumerator())
                using (var validators = this.argumentValidators.GetEnumerator())
                {
                    while (callArguments.MoveNext() && validators.MoveNext())
                    {
                        if (!validators.Current.IsValid(callArguments.Current))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            public override string ToString()
            {
                return "{0}.{1}({2})".FormatInvariant(this.method.DeclaringType.FullName, this.method.Name, this.GetParametersString());
            }

            private string GetParametersString()
            {
                var result = new StringBuilder();

                using (var parameters = this.method.GetParameters().Cast<ParameterInfo>().GetEnumerator())
                using (var validators = this.argumentValidators.GetEnumerator())
                {
                    while (parameters.MoveNext() && validators.MoveNext())
                    {
                        if (result.Length > 0)
                        {
                            result.Append(", ");
                        }

                        result.AppendFormat("[{0}] {1}", parameters.Current.ParameterType, validators.Current.ToString());
                    }
                }

                return result.ToString();
            }
        }

        private class PropertyRuleState
            : RuleState
        {
            private MethodRuleState getMethodRule;
            private PropertyInfo property;

            public PropertyRuleState(MemberExpression expression)
            {
                this.property = expression.Member as PropertyInfo;

                var getMethod = this.property.GetGetMethod();
                this.getMethodRule = new MethodRuleState(getMethod, Enumerable.Empty<IArgumentValidator>());
            }

            public override bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return this.getMethodRule.IsApplicableTo(fakeObjectCall);
            }

            public override string ToString()
            {
                return "{0}.{1}".FormatInvariant(this.property.DeclaringType.FullName, this.property.Name);
            }
        }
        #endregion
    }
}