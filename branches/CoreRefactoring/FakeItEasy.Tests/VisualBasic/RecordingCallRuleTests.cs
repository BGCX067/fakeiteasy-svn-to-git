using NUnit.Framework;
using FakeItEasy.VisualBasic;
using FakeItEasy.Api;
using FakeItEasy.Tests.TestHelpers;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy.Extensibility;
using System;

namespace FakeItEasy.Tests.VisualBasic
{
    [TestFixture]
    public class RecordingCallRuleTests
    {
        IFoo fakedObject;
        FakeObject fakeObject;
        RecordedCallRule recordedRule;
        FakeAsserter.Factory asserterFactory;
        IEnumerable<IFakeObjectCall> argumentUsedForAsserterFactory;
        FakeAsserter asserter;
        IExtensibleIs Its;

        [SetUp]
        public void SetUp()
        {
            this.fakedObject = A.Fake<IFoo>();
            this.fakeObject = Fake.GetFakeObject(fakedObject);
            this.recordedRule = A.Fake<RecordedCallRule>(() => new RecordedCallRule(A.Fake<MethodInfoManager>()));

            this.asserter = A.Fake<FakeAsserter>(() => new FakeAsserter(Enumerable.Empty<IFakeObjectCall>()));

            this.asserterFactory = x =>
                {
                    this.argumentUsedForAsserterFactory = x;
                    return this.asserter;
                };
        }

        [TearDown]
        public void TearDown()
        {
            this.argumentUsedForAsserterFactory = null;
        }

        private RecordingCallRule<IFoo> CreateRule()
        {
            return new RecordingCallRule<IFoo>(this.fakeObject, this.recordedRule, this.asserterFactory);
        }

        [Test]
        public void Apply_should_create_asserter_and_call_it_with_the_recorded_call_when_IsAssertion_is_set_to_true_on_recorded_rule()
        {
            this.recordedRule.IsAssertion = true;

            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();
            Fake.Configure(call)
                .CallsTo(x => x.ToString())
                .Returns("call description");

            rule.Apply(call);

            Fake.Assert(this.asserter)
                .WasCalled(x => x.AssertWasCalled(Its.Any<Func<IFakeObjectCall, bool>>(), "call description", Its.Any<Func<int, bool>>(), "the number of times specified by predicate"));
        }

        [Test]
        public void Apply_should_create_asserter_and_call_it_with_call_predicate_that_invokes_IsApplicableTo_on_the_recorded_rule()
        {
            this.recordedRule.IsAssertion = true;

            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();
            Fake.Configure(call)
                .CallsTo(x => x.ToString())
                .Returns("call description");

            rule.Apply(call);

            var asserterCall = Fake.GetCalls(this.asserter).Matching(x => x.AssertWasCalled(Its.Any<Func<IFakeObjectCall, bool>>(), "call description", Its.Any<Func<int, bool>>(), Its.Any<string>())).Single();
            var callPredicate = asserterCall.Arguments.Get<Func<IFakeObjectCall, bool>>("callPredicate");

            callPredicate.Invoke(call);

            Fake.Assert(this.recordedRule)
                .WasCalled(x => x.IsApplicableTo(call));
        }

        [Test]
        public void Apply_should_call_asserter_with_repeat_predicate_from_recorded_rule()
        {
            Func<int, bool> repeatPredicate = x => true;
            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatPredicate = repeatPredicate;

            this.recordedRule.IsAssertion = true;

            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();
            Fake.Configure(call)
                .CallsTo(x => x.ToString())
                .Returns("call description");

            rule.Apply(call);

            var asserterCall = Fake.GetCalls(this.asserter).Matching(x => x.AssertWasCalled(Its.Any<Func<IFakeObjectCall, bool>>(), "call description", Its.Any<Func<int, bool>>(), Its.Any<string>())).Single();
            var repeatPredicatePassedToAsserter = asserterCall.Arguments.Get<Func<int, bool>>("repeatPredicate");

            Assert.That(repeatPredicatePassedToAsserter, Is.SameAs(repeatPredicate));
        }

        [Test]
        public void Apply_should_pass_calls_from_fake_object_to_fake_asserter_factory()
        {
            this.fakedObject.Bar();
            this.fakedObject.Baz();

            this.recordedRule.IsAssertion = true;
            this.recordedRule.RepeatPredicate = x => true;

            var rule = this.CreateRule();

            rule.Apply(A.Fake<IWritableFakeObjectCall>());

            Assert.That(this.argumentUsedForAsserterFactory, Is.EquivalentTo(this.fakeObject.RecordedCallsInScope));
        }

        [Test]
        public void NumberOfTimesToCall_should_be_one()
        {
            var rule = this.CreateRule();

            Assert.That(rule.NumberOfTimesToCall, Is.EqualTo(1));
        }
    }

    [TestFixture]
    public class RecordedCallRuleTests
    {
        private RecordedCallRule CreateRule()
        {
            return new RecordedCallRule(A.Fake<MethodInfoManager>());
        }

        [Test]
        public void UsePredicateToValidateArguments_should_set_predicate_to_IsApplicableToArguments()
        {
            var rule = this.CreateRule();

            Func<ArgumentCollection, bool> predicate = x => true;

            rule.UsePredicateToValidateArguments(predicate);

            Assert.That(rule.IsApplicableToArguments, Is.SameAs(predicate));
        }
    }
}
