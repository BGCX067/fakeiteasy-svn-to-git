using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Extensibility;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.Expressions.ArgumentValidators
{
    public abstract class ArgumentValidatorTestBase
    {
        protected IArgumentValidator Validator;
        protected abstract IEnumerable<object> InvalidValues { get; }
        protected abstract IEnumerable<object> ValidValues { get; }
        protected abstract string ExpectedDescription { get; }

        [Test]
        [TestCaseSource("InvalidValues")]
        public void IsValid_should_return_false_for_invalid_values(object invalidValue)
        {
            Assert.That(this.Validator.IsValid(invalidValue), Is.False);
        }

        [Test]
        [TestCaseSource("ValidValues")]
        public void IsValid_should_return_true_for_valid_values(object validValue)
        {
            Assert.That(this.Validator.IsValid(validValue), Is.True);
        }

        [Test]
        public void Validator_should_provide_correct_description()
        {
            Assert.That(this.Validator.ToString(), Is.EqualTo(this.ExpectedDescription));
        }
    }
}
