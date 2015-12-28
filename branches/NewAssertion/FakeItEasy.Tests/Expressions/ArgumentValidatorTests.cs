namespace FakeItEasy.Tests.Expressions
{
    using NUnit.Framework;
    using FakeItEasy.Expressions;
    using FakeItEasy.Tests.Expressions.ArgumentValidators;
    using System;

    [TestFixture]
    public class ArgumentValidatorTests
        : ArgumentValidatorTestBase<int>
    {
        private ArgumentValidations<int> validations;

        [SetUp]
        public void SetUp()
        {
            this.validations = A.Fake<ArgumentValidations<int>>();
            A.CallTo(() => this.validations.IsValid(A<int>.Ignored)).Returns(true);

            this.Validator = new TestableValidator(this.validations);
        }

        private TestableValidator CreateValidator()
        {
            return new TestableValidator(this.validations);
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

            var validations = A.Fake<ArgumentValidations<string>>();
            A.CallTo(() => validations.IsValid(A<string>.Ignored)).Returns(true);
            
            // Act
            var validator = ArgumentValidator<string>.Create(validations, predicate, "foo");
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
            var validator = ArgumentValidator<int>.Create(this.validations, predicate, "foo");
            var isValid = validator.IsValid(1);

            // Assert
            Assert.That(isValid, Is.EqualTo(predicateResponse));
        }

        [Test]
        public void Create_should_return_validator_that_prints_description_from_ToString()
        {
            // Arrange
            var validator = ArgumentValidator<int>.Create(this.validations, x => true, "Any Int32");

            // Act
            var description = validator.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("<Any Int32>"));
        }

        [Test]
        public void Create_should_return_validator_with_the_passed_in_validations_object()
        {
            // Arrange
            var validations = A.Fake<ArgumentValidations<int>>();
            
            // Act
            var validator = ArgumentValidator<int>.Create(validations, x => true, "foo");

            // Assert
            Assert.That(validator.Validations, Is.SameAs(validations));
        }

        [Test]
        public void Create_should_be_null_guarded()
        {
            // Assert
            NullGuardedConstraint.Assert(() =>
                ArgumentValidator<int>.Create(this.validations, x => true, "foo"));
        }

        [Test]
        public void Create_should_throw_when_description_is_an_empty_string()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
                ArgumentValidator<int>.Create(this.validations, x => true, string.Empty));
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

        [Test]
        public void IsValid_should_return_false_when_the_validator_is_valid_in_itself_but_the_parent_validations_is_not_valid()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = true;

            A.CallTo(() => this.validations.IsValid(A<int>.Ignored)).Returns(false);
            
            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsValid_should_return_true_when_validator_is_not_valid_and_parent_validations_is_not_valid()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = false;

            A.CallTo(() => this.validations.IsValid(A<int>.Ignored)).Returns(false);

            // Act
            var result = validator.IsValid(10);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void And_should_return_validations_that_delegates_IsValid_to_IsValid_of_validator(bool validatorIsValid)
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.EvaluateReturnValue = validatorIsValid;

            A.CallTo(() => this.validations.IsValid(10)).Returns(true);

            // Act
            var result = validator.And;

            // Assert
            Assert.That(result.IsValid(10), Is.EqualTo(validatorIsValid));
        }

        [Test]
        public void And_should_return_validations_that_concatenates_the_full_description_of_the_parent_validator_with_AND()
        {
            // Arrange
            var validator = this.CreateValidator();
            validator.DescriptionToUse = "foo";

            // Act
            var description = validator.And.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("foo and"));
        }

        [TestCase("foo and", "bar", Result = "foo and bar")]
        [TestCase("", "something", Result = "something")]
        [TestCase(null, "foo", Result = "foo")]
        public string FullDescription_should_return_description_of_validations_concatenated_with_own_description(string validationsDescription, string validatorDescription)
        {
            // Arrange
            A.CallTo(() => this.validations.ToString()).Returns(validationsDescription);

            var validator = this.CreateValidator();
            validator.DescriptionToUse = validatorDescription;

            // Act
            var description = validator.FullDescription;

            // Assert
            return description;
        }

        [TestCase(true, true, Result = true)]
        [TestCase(true, false, Result = true)]
        [TestCase(false, true, Result = true)]
        [TestCase(false, false, Result = false)]
        public bool Or_should_return_validator_that_is_valid_if_any_of_the_validators_are_valid(bool firstValidatorResult, bool secondValidatorResult)
        {
            // Arrange
            A.CallTo(() => this.validations.IsValid(3)).Returns(true);

            var firstValidator = this.CreateValidator();
            firstValidator.EvaluateReturnValue = firstValidatorResult;

            var secondValidator = this.CreateValidator();
            secondValidator.EvaluateReturnValue = secondValidatorResult;

            // Act
            var orValidator = firstValidator.Or(secondValidator);

            // Assert
            return orValidator.IsValid(3);
        }

        [Test]
        public void Or_should_return_validator_that_combines_the_descriptions_of_the_two_validators()
        {
            // Arrange
            var firstValidator = this.CreateValidator();
            firstValidator.DescriptionToUse = "first";

            var secondValidator = this.CreateValidator();
            secondValidator.DescriptionToUse = "second";

            var orValidator = firstValidator.Or(secondValidator);

            // Act

            // Assert
            Assert.That(orValidator.FullDescription, Is.EqualTo("first or (second)"));
        }

        private class TestableValidator
            : ArgumentValidator<int>
        {
            public bool EvaluateReturnValue = true;

            public TestableValidator(ArgumentValidations<int> validations) : base(validations) { }

            public string DescriptionToUse = "";

            protected override string Description
            {
                get { return this.DescriptionToUse; }
            }

            protected override bool Evaluate(int value)
            {
                return this.EvaluateReturnValue;
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
            get { return ""; }
        }
    }
}
