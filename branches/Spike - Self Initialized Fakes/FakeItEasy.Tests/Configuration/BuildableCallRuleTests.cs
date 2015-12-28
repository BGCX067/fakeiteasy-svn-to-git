using System;
using FakeItEasy.Configuration;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework.Constraints;
using FakeItEasy.Api;

namespace FakeItEasy.Tests.Configuration
{
    [TestFixture]
    public class BuildableCallRuleTests
    {
        private BuildableCallRule CreateRule()
        {
            return new TestableCallRule();
        }

        [Test]
        public void Apply_should_invoke_all_actions_in_the_actions_collection()
        {
            // Arrange
            var rule = this.CreateRule();

            bool firstWasCalled = false;
            bool secondWasCalled = false;
            rule.Actions.Add(x => firstWasCalled = true);
            rule.Actions.Add(x => secondWasCalled = true);

            rule.Applicator = x => { };

            // Act
            rule.Apply(A.Fake<IWritableFakeObjectCall>());

            // Assert
            Assert.That(firstWasCalled, Is.True);
            Assert.That(secondWasCalled, Is.True);
        }

        [Test]
        public void Apply_should_pass_the_call_to_specified_actions()
        {
            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();
            IFakeObjectCall passedCall = null;

            rule.Applicator = x => { };
            rule.Actions.Add(x => passedCall = x);

            rule.Apply(call);

            Assert.That(passedCall, Is.SameAs(call));
        }

        [Test]
        public void Apply_should_call_CallBaseMethod_on_intercepted_call_when_CallBaseMethod_is_set_to_true()
        {
            var rule = this.CreateRule();

            var call = A.Fake<IWritableFakeObjectCall>();

            rule.Applicator = x => { };
            rule.CallBaseMethod = true;
            rule.Apply(call);

            Fake.Assert(call).WasCalled(x => x.CallBaseMethod());
        }

        private class TestableCallRule
            : BuildableCallRule
        {
            protected override bool OnIsApplicableTo(IFakeObjectCall fakeObjectCall)
            {
                return true;
            }
        }

    }
}
