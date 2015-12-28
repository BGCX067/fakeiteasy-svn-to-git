using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Api;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Extensibility;

namespace FakeItEasy.Tests.Expressions
{
    [TestFixture]
    public class ExpressionCallRuleTests
    {
        private ExpressionCallMatcher callMatcher;

        [SetUp]
        public void SetUp()
        {
            Expression<Action<IFoo>> dummyCall = x => x.Bar();
            this.callMatcher = A.Fake<ExpressionCallMatcher>(() => new ExpressionCallMatcher(dummyCall, A.Fake<ArgumentValidatorFactory>()));
        }

        private ExpressionCallRule CreateRule<T>(Expression<Action<T>> callSpecification)
        {
            return new ExpressionCallRule(this.callMatcher);
        }

        private ExpressionCallRule CreateRule<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new ExpressionCallRule(this.callMatcher);
        }


        [Test]
        public void IsApplicableTo_should_pass_call_to_call_matcher()
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            var rule = this.CreateRule<IFoo>(x => x.Bar());

            rule.IsApplicableTo(call);

            Fake.Assert(this.callMatcher)
                .WasCalled(x => x.Matches(call));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsApplicableTo_should_return_result_from_call_matcher(bool callMatcherResult)
        {
            var call = ExpressionHelper.CreateFakeCall<IFoo>(x => x.Bar());

            Fake.Configure(this.callMatcher)
                .CallsTo(x => x.Matches(call))
                .Returns(callMatcherResult);

            var rule = this.CreateRule<IFoo>(x => x.Bar());

            var result = rule.IsApplicableTo(call);

            Assert.That(result, Is.EqualTo(callMatcherResult));
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new ExpressionCallRule(this.callMatcher));
        }

      
        [Test]
        public void Apply_should_call_the_applicator_with_the_incoming_call()
        {
            IWritableFakeObjectCall callPassedToApplicator = null;
            var callPassedToRule = FakeCall.Create<IFoo>("Bar");

            var rule = CreateRule<IFoo>(x => x.Bar());
            rule.Applicator = x => callPassedToApplicator = x;

            rule.Apply(callPassedToRule);

            Assert.That(callPassedToApplicator, Is.SameAs(callPassedToRule));
        }

        [Test]
        public void Apply_should_throw_when_applicator_is_null()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());

            Assert.Throws<InvalidOperationException>(
                () => rule.Apply(new FakeCall()));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_settable_and_gettable()
        {
            var rule = CreateRule<IFoo>(x => x.Bar());
            rule.NumberOfTimesToCall = 10;

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(10));
        }

        [Test]
        public void ToString_should_return_expressionMatcher_ToString()
        {
            Fake.Configure(this.callMatcher)
                .CallsTo(x => x.ToString())
                .Returns("foo");

            var rule = CreateRule<IFoo>(x => x.Bar());

            Assert.That(rule.ToString(), Is.EqualTo("foo"));
        }  
    }
}