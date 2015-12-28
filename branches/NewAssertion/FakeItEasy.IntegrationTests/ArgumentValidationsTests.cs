using NUnit.Framework;
using FakeItEasy.Expressions;
using FakeItEasy.Tests;
using System;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ArgumentValidationsTests
    {
        [Test]
        public void Not_should_reverse_IsValid()
        {
            // Arrange
            var validator = A<string>.That.Not.IsNull();

            // Act

            // Assert
            Assert.That(validator.IsValid(null), Is.False);
            Assert.That(validator.IsValid("foo"), Is.True);
        }

        [Test]
        public void Validators_should_be_combinable_by_and()
        {
            // Arrange
            var validator = A<object>.That.Not.IsNull().And.IsInstanceOf<string>().And.Matches(_ => ((string)_).StartsWith("foo"));

            // Act
            
            // Assert
            Assert.That(validator.IsValid("foo bar"));
        }

        [Test]
        public void Validators_should_be_combinable_by_or()
        {
            // Arrange
            var validator = A<string>.That.StartsWith("foo").Or(A<string>.That.StartsWith("bar"));

            // Act

            // Assert
            Assert.That(validator.IsValid("foo..."));
            Assert.That(validator.IsValid("bar..."));
        }
    }

    public static class StringArgumentValidations
    {
        public static ArgumentValidator<string> StartsWith(this ArgumentValidations<string> validations, string beginning)
        {
            return ArgumentValidator<string>.Create(validations, x => x.StartsWith(beginning), string.Format("Starts with \"{0}\"", beginning));
        }
    }
}
