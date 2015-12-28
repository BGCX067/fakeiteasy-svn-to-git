﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Extensibility;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests.Expressions.ArgumentValidators
{
    [TestFixture]
    public class EqualityArgumentValidatorTests
        : ArgumentValidatorTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = new EqualityArgumentValidator(1);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get 
            {
                return new[] { null, new object(), Guid.NewGuid(), "FOO", " foo " }; 
            }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { 1 }; }
        }

        protected override string ExpectedDescription
        {
            get { return "1"; }
        }

        [Test]
        public override void Validator_should_provide_correct_description()
        {
            Assert.That(this.Validator.ToString(), Is.EqualTo("1"));
        }

        [Test]
        public void ToString_should_return_NULL_when_expected_value_is_null()
        {
            var validator = new EqualityArgumentValidator(null);

            Assert.That(validator.ToString(), Is.EqualTo("<NULL>"));
        }

        [Test]
        public void ToString_should_put_accents_when_expected_value_is_string()
        {
            var validator = new EqualityArgumentValidator("foo");

            Assert.That(validator.ToString(), Is.EqualTo("\"foo\""));
        }
    }
}