namespace FakeItEasy.Tests.Expressions
{
    using FakeItEasy.Expressions;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class ArgumentValidationsTests
    {
        private ArgumentValidations<T> CreateValidations<T>()
        {
            return new ArgumentValidations<T>();
        }

        [TestCase(10, Result = true)]
        [TestCase(9, Result = false)]
        public bool Matches_should_return_validator_that_delegates_to_predicate(int value)
        {
            // Arrange
            Func<int, bool> predicate = x => x == 10;
            var validations = this.CreateValidations<int>();

            // Act
            
            // Assert
            return validations.Matches(predicate).IsValid(value);
        }

        [Test]
        public void Matches_should_return_validator_that_has_correct_description()
        {
            // Arrange
            var validations = this.CreateValidations<int>();

            // Act
            var validator = validations.Matches(x => true);

            // Assert
            Assert.That(validator.ToString(), Is.EqualTo("<Predicate>"));
        }
    }
}
