using System;
using System.Collections.Generic;
using FakeItEasy.Expressions;
using NUnit.Framework;

namespace FakeItEasy.Tests.Expressions.ArgumentValidators
{
    [TestFixture]
    public class AnyValidatorTests
        : ArgumentValidatorTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = new AnyValidator<string>();
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { 1, new DateTime(), new object() }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { null, "", "foo" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "<Any String>"; }
        }
    }
}
