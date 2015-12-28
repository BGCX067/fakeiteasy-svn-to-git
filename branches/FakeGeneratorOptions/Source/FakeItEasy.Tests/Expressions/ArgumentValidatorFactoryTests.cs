namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.TestHelpers;
    using NUnit.Framework;

    [TestFixture]
    public class ArgumentValidatorFactoryTests
    {
        private ArgumentValidatorFactory CreateFactory()
        {
            return new ArgumentValidatorFactory();
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_produced_by_method()
        {
            // Arrange
            var validator = A.Fake<ArgumentValidator<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(GetValidator(validator)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentValidator(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_passed_in_directly()
        {
            // Arrange
            var validator = A.Fake<ArgumentValidator<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(validator), 0);
            
            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentValidator(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_when_Argument_property_has_been_called_on_it()
        {
            // Arrange
            var validator = A.Fake<ArgumentValidator<int>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Foo(validator.Argument), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentValidator(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        [Test]
        public void GetArgumentValidator_should_get_validator_when_parameter_is_of_type_object()
        {
            // Arrange
            var validator = A.Fake<ArgumentValidator<object>>();
            var argument = ExpressionHelper.GetArgumentExpression<ITestInterface>(x => x.Bar(GetValidator(validator)), 0);

            // Act
            var factory = this.CreateFactory();
            var producedValidator = factory.GetArgumentValidator(argument);

            // Assert
            Assert.That(producedValidator, Is.SameAs(validator));
        }

        private static ArgumentValidator<T> GetValidator<T>(ArgumentValidator<T> validator)
        {
            return validator;
        }

        public interface ITestInterface
        {
            void Foo(int number);
            void Bar(object value);
        }
    }
}
