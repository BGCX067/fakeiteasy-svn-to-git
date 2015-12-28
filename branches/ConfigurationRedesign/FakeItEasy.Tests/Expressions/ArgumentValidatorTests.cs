namespace FakeItEasy.Tests.Expressions
{
    using NUnit.Framework;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.Expressions.ArgumentValidators;
    using System;

    [TestFixture]
    public class ArgumentValidatorTests
        : ArgumentValidatorTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = new TestableValidator();
        }

        private TestableValidator CreateValidator()
        {
            return new TestableValidator();
        }

        [Test]
        public void Create_should_return_validator_that_calls_predicate_when_IsValid_is_called()
        {
            // Arrange
            string argumentForPredicate = null;

            Func<string, bool> predicate = x =>
                {
                    argumentForPredicate = x;
                    return true;
                };

            // Act
            var validator = ArgumentValidator<string>.Create(predicate, "foo");
            validator.IsValid("argument");

            // Assert
            Assert.That(argumentForPredicate, Is.EqualTo("argument"));
        }

        [Test]
        public void Create_should_return_validator_that_delegates_response_from_predicate(
            [Values(true, false)] bool predicateResponse)
        {
            // Arrange
            Func<int, bool> predicate = x => predicateResponse;

            // Act
            var validator = ArgumentValidator<int>.Create(predicate, "foo");
            var isValid = validator.IsValid(1);

            // Assert
            Assert.That(isValid, Is.EqualTo(predicateResponse));
        }

        [Test]
        public void Create_should_return_validator_that_prints_description_from_ToString()
        {
            // Arrange
            var validator = ArgumentValidator<int>.Create(x => true, "Any Int32");

            // Act
            var description = validator.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("<Any Int32>"));
        }

        [Test]
        public void Create_should_be_null_guarded()
        {
            // Assert
            NullGuardedConstraint.Assert(() =>
                ArgumentValidator<int>.Create(x => true, "foo"));
        }

        [Test]
        public void Create_should_throw_when_description_is_an_empty_string()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                ArgumentValidator<int>.Create(x => true, string.Empty));
        }

        [Test]
        public void ToString_should_return_formatted_description()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.DescriptionToUse = "Some Description";

            // Act
            var description = validator.ToString();

            // Assert
            Assert.That(validator.ToString(), Is.EqualTo("<Some Description>"));
        }

        private class TestableValidator
            : ArgumentValidator<int>
        {
            public string DescriptionToUse = "";

            public override bool IsValid(int value)
            {
                return true;
            }

            protected override string Description
            {
                get { return this.DescriptionToUse; }
            }
        }

        protected override System.Collections.Generic.IEnumerable<object> InvalidValues
        {
            get { return new object[] { }; }
        }

        protected override System.Collections.Generic.IEnumerable<object> ValidValues
        {
            get { return new object[] { 1, 2, 3 }; }
        }

        protected override string ExpectedDescription
        {
            get { return "<>"; }
        }
    }
}
