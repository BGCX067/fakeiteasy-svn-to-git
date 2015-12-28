using System.Collections.Generic;
using FakeItEasy.Expressions;
using NUnit.Framework;

namespace FakeItEasy.Tests.Expressions.ArgumentValidators
{
    [TestFixture]
    public class PredicateValidatorTests
        : ArgumentValidatorTestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Validator = new PredicateValidator<string>(x => x.Length > 1);
        }

        protected override IEnumerable<object> InvalidValues
        {
            get { return new object[] { "", "a" }; }
        }

        protected override IEnumerable<object> ValidValues
        {
            get { return new object[] { "foo", "bar", "something longer" }; }
        }

        protected override string ExpectedDescription
        {
            get { return "<predicate System.String>"; }
        }
    }
}
