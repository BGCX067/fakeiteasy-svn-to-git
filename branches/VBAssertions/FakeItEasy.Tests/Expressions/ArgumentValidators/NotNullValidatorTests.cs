using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Expressions.ArgumentValidators;

namespace FakeItEasy.Tests.Expressions.ArgumentValidators
{
    [TestFixture]
    public class NotNullValidatorTests
        : ArgumentValidatorTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = new NotNullValidator();
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { null }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new[] { 1, string.Empty, new object() }; }
        }

        protected override string ExpectedDescription
        {
            get { return "<NOT NULL>"; }
        }
    }
}
