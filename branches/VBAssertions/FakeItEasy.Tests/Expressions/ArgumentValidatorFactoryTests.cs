namespace FakeItEasy.Tests.Expressions
{
    using NUnit.Framework;
using FakeItEasy.Expressions;
using System.Linq.Expressions;
using System;
    using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Extensibility;

    [TestFixture]
    public class ArgumentValidatorFactoryTests
    {
        private ArgumentValidatorFactory CreateFactory()
        {
            return new ArgumentValidatorFactory();
        }

        [Test]
        public void GetArgumentValidator_should_return_EqualityArgumentValidator_when_expression_is_not_method_call(
            [Values("", 1, 3, float.MaxValue, null)]object value)
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(value), 0);
            var factory = this.CreateFactory();

            var result = factory.GetArgumentValidator(argument);

            Assert.That(result.IsValid(value), Is.True);
            Assert.That(result.IsValid(new object()), Is.False);
        }

        [Test]
        public void GetArgumentValidator_should_return_equality_validator_when_expression_is_method_call_thats_not_attributed()
        {
            var value = string.Empty;
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(value.ToString()), 0);

            var factory = this.CreateFactory();
            var result = factory.GetArgumentValidator(argument);

            Assert.That(result.IsValid(value), Is.True);
            Assert.That(result.IsValid(new object()), Is.False);
        }

        [Test]
        public void GetArgumentValidator_should_create_specified_validator_type()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(Simple()), 0);

            var factory = this.CreateFactory();
            var result = factory.GetArgumentValidator(argument);

            Assert.That(result, Is.InstanceOf<SimpleValidator>());
        }

        [Test]
        public void GetArgumentValidator_should_pass_method_arguments_to_validator_constructor()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(TakesArgumentsForConstructor("foo", 10)), 0);

            var factory = this.CreateFactory();
            var validator = factory.GetArgumentValidator(argument) as ValidatorThatTakesArgumentsForConstructor;

            Assert.That(validator.First, Is.EqualTo("foo"));
            Assert.That(validator.Second, Is.EqualTo(10));
        }

        [Test]
        public void GetArgumentValidator_should_pass_method_arguments_to_validator_constructor_omitting_first_argument_when_its_an_IExtensibleIs()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(TakesArgumentsForConstructor((IExtensibleIs)null, "foo", 10)), 0);

            var factory = this.CreateFactory();
            var validator = factory.GetArgumentValidator(argument) as ValidatorThatTakesArgumentsForConstructor;

            Assert.That(validator.First, Is.EqualTo("foo"));
            Assert.That(validator.Second, Is.EqualTo(10));
        }

        [Test]
        public void GetArgumentValidator_should_use_generic_argument_to_method_for_validator_that_takes_generic_argument()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(GenericValidator<object>()), 0);

            var factory = this.CreateFactory();
            var validator = factory.GetArgumentValidator(argument);

            Assert.That(validator, Is.InstanceOf<ValidatorWithGenericArgument<object>>());
        }

        [Test]
        public void GetArgumentValidator_should_be_able_to_get_argument_validator_when_argument_is_boxed()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar(GenericValidator<int>()), 0);

            var factory = this.CreateFactory();
            var validator = factory.GetArgumentValidator(argument);

            Assert.That(validator, Is.InstanceOf<ValidatorWithGenericArgument<int>>());
        }

        [Test]
        public void GetArgumentValidator_should_be_able_to_get_argument_validator_when_argument_is_casted()
        {
            var argument = ExpressionHelper.GetArgumentExpression<IFoo>(x => x.Bar((string)GenericValidator<object>()), 0);

            var factory = this.CreateFactory();
            var validator = factory.GetArgumentValidator(argument);

            Assert.That(validator, Is.InstanceOf<ValidatorWithGenericArgument<object>>());
        }

        [ArgumentValidator(typeof(SimpleValidator))]
        private string Simple()
        {
            throw new NotSupportedException();
        }

        private class SimpleValidator
            : IArgumentValidator
        {
            public bool IsValid(object argument)
            {
                return true;
            }
        }

        [ArgumentValidator(typeof(ValidatorThatTakesArgumentsForConstructor))]
        private object TakesArgumentsForConstructor(IExtensibleIs extensionPoint, object first, object second)
        {
            throw new NotSupportedException();
        }

        [ArgumentValidator(typeof(ValidatorThatTakesArgumentsForConstructor))]
        private object TakesArgumentsForConstructor(object first, object second)
        {
            throw new NotSupportedException();
        }

        private class ValidatorThatTakesArgumentsForConstructor
            : IArgumentValidator
        {
            public object First;
            public object Second;

            public ValidatorThatTakesArgumentsForConstructor(object first, object second)
            {
                this.First = first;
                this.Second = second;
            }

            public bool IsValid(object argument)
            {
                throw new NotImplementedException();
            }
        }

        [ArgumentValidator(typeof(ValidatorWithGenericArgument<>))]
        private T GenericValidator<T>()
        {
            throw new NotSupportedException();
        }

        private class ValidatorWithGenericArgument<T>
            : IArgumentValidator
        {
            public bool IsValid(object argument)
            {
                throw new NotImplementedException();
            }
        }
    }
}
