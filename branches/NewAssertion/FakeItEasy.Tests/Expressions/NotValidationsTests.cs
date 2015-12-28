namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class NotValidationsTests
    {
        private ArgumentValidations<string> parentValidations;

        [SetUp]
        public void SetUp()
        {
            this.parentValidations = A.Fake<ArgumentValidations<string>>();
            A.CallTo(() => this.parentValidations.IsValid(A<string>.Ignored)).Returns(true);
        }

        private NotValidations<string> CreateValidations()
        {
            return new NotValidations<string>(this.parentValidations);
        }

        [Test]
        public void IsValid_should_return_false_when_parent_validations_is_valid()
        {
            // Arrange
            A.CallTo(() => this.parentValidations.IsValid("foo")).Returns(true);
            var validations = this.CreateValidations();

            // Act
            
            // Assert
            Assert.That(validations.IsValid("foo"), Is.False);
        }

        [Test]
        public void IsValid_should_return_true_when_parent_validations_is_not_valid()
        {
            // Arrange
            A.CallTo(() => this.parentValidations.IsValid("foo")).Returns(false);
            var validations = this.CreateValidations();

            // Act

            // Assert
            Assert.That(validations.IsValid("foo"), Is.True);
        }

        [TestCase("", Result = "not")]
        [TestCase("Foo AND", Result = "Foo AND not")]
        public string ToString_should_return_parent_validations_ToString_concatenated_with_NOT(string parentToString)
        {
            // Arrange
            A.CallTo(() => this.parentValidations.ToString()).Returns(parentToString);
            var validations = this.CreateValidations();

            // Act

            // Assert
            return validations.ToString();
        }
    }
}
