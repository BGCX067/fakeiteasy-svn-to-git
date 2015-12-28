using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.VisualBasic;
using FakeItEasy.Api;
using FakeItEasy.Assertion;
using System.Linq.Expressions;

namespace FakeItEasy.Tests.VisualBasic
{
    [TestFixture]
    public class ThisCallTests
    {
        private ServiceLocator replacedServiceLocator;

        [SetUp]
        public void SetUp()
        {
            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(ServiceLocator.Current);
        }

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
        }

        [Test]
        public void Throws_should_throw_exception_when_configured_method_is_called_but_not_while_recording()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).Throws(new ApplicationException());
            fake.Bar();

            Assert.Throws<ApplicationException>(() =>
                fake.Bar());
        }

        [Test]
        public void Recorded_calls_should_only_be_applicable_to_the_method_called()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).Throws(new Exception("This should not be thrown!"));
            fake.Bar();

            fake.Baz();
        }

        [Test]
        public void Recording_a_rule_should_work_even_when_there_are_already_call_rules_specified()
        {
            var fake = A.Fake<IFoo>();

            Configure.Fake(fake).CallsTo(x => x.Baz()).Returns(10);
            Configure.Fake(fake).CallsTo(x => x.Biz()).Returns("something");
            
            ThisCall.To(fake).Throws(new ApplicationException());
            fake.Bar();

            Assert.Throws<ApplicationException>(() =>
                fake.Bar());
        }

        [Test]
        public void Recorded_call_should_not_be_applied_when_arguments_are_not_the_same_as_used_when_recording()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).Throws(new AssertionException("This should not be thrown!"));
            fake.Baz(1, 2);

            Assert.That(fake.Baz("foo", "bar"), Is.Not.EqualTo(10));
        }

        [Test]
        public void Recorded_call_should_not_be_applied_when_arguments_does_not_match_specified_argument_predicate()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).WhenArgumentsMatch(x => false).Throws(new AssertionException("This should not happen"));
            fake.Bar(10);

            fake.Bar(10);
        }

        [Test]
        public void Recorded_call_should_apply_when_argument_predicate_returns_true()
        {
            var fake = A.Fake<IFoo>();

            ThisCall.To(fake).WhenArgumentsMatch(x => true).Throws(new ArgumentNullException());
            fake.Bar(10);

            Assert.Throws<ArgumentNullException>(() =>
                fake.Bar(10));
        }

        [Test]
        public void Arguments_from_the_call_should_be_sent_to_the_argument_predicate()
        {
            var fake = A.Fake<IFoo>();
            ArgumentCollection arguments = null;

            ThisCall.To(fake).WhenArgumentsMatch(x => { arguments = x; return true; }).Throws(new ApplicationException());
            fake.Baz(null, null);

            try
            {
                fake.Baz(1, "a");
            }
            catch (ApplicationException) { }
            
            Assert.That(arguments.AsEnumerable().ToArray(), Has.Some.EqualTo(1).And.Some.EqualTo("a"));
        }


        [Test]
        public void To_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => ThisCall.To(A.Fake<IFoo>()));
        }
    }
}
