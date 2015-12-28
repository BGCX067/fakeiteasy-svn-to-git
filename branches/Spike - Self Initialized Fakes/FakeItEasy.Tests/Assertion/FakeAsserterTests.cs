using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using FakeItEasy.Assertion;

namespace FakeItEasy.Tests.Assertion
{
    [TestFixture]
    public class FakeAsserterTests
    {
        private FakeObject fake;
        private FakeAsserter<IFoo> asserter;

        [SetUp]
        public void SetUp()
        {
            this.fake = new FakeObject(typeof(IFoo));
            this.asserter = new FakeAsserter<IFoo>(this.fake);
        }

        private IFoo FakedFoo
        {
            get
            {
                return (IFoo)this.fake.Object;
            }
        }

        [Test]
        public void Constructor_throws_when_fake_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new FakeAsserter<IFoo>((FakeObject)null));
        }

        [Test]
        public void WasCalled_with_void_call_should_throw_when_call_was_not_made()
        {
            Assert.Throws<ExpectationException>(() =>
                this.asserter.WasCalled(x => x.Bar()));
        }

        [Test]
        public void WasCalled_with_void_call_should_not_throw_when_call_has_been_made()
        {
            this.FakedFoo.Bar();

            this.asserter.WasCalled(x => x.Bar());
        }

        [Test]
        public void WasCalled_with_return_value_call_should_throw_when_call_was_not_made()
        {
            Assert.Throws<ExpectationException>(() =>
                this.asserter.WasCalled(x => x.Baz()));
        }

        [Test]
        public void WasCalled_with_return_value_call_should_not_throw_when_call_has_been_made()
        {
            this.FakedFoo.Baz();

            this.asserter.WasCalled(x => x.Baz());
        }

        [Test]
        public void WasNotCalled_with_void_call_should_throw_when_call_has_been_made()
        {
            this.FakedFoo.Bar();

            Assert.Throws<ExpectationException>(() =>
                this.asserter.WasNotCalled(x => x.Bar()));
        }

        [Test]
        public void WasNotCalled_with_void_call_should_not_throw_when_call_has_not_been_made()
        {
            this.asserter.WasNotCalled(x => x.Bar());
        }

        [Test]
        public void WasNotCalled_with_function_call_should_throw_when_call_has_been_made()
        {
            this.FakedFoo.Baz();

            Assert.Throws<ExpectationException>(() =>
                this.asserter.WasNotCalled(x => x.Baz()));
        }

        [Test]
        public void WasNotCalled_with_function_call_should_not_throw_when_call_has_not_been_made()
        {
            this.asserter.WasNotCalled(x => x.Baz());
        }

        [Test]
        public void WasCalled_with_repeat_predicate_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.asserter.WasCalled(x => x.Bar(), x => x > 10));
        }

        [Test]
        public void WasCalled_should_not_throw_when_method_was_called_and_predicate_returns_true()
        {
            this.FakedFoo.Baz();

            this.asserter.WasCalled(x => x.Bar(), x => true);
        }

        [Test]
        public void WasCalled_with_void_call_should_throw_when_predicate_returns_false()
        {
            this.FakedFoo.Baz();
            
            Assert.Throws<ExpectationException>(() =>            
                this.asserter.WasCalled(x => x.Bar(), x => false));
        }

        [Test]
        public void WasCalled_for_void_calls_should_pass_the_number_of_times_a_method_was_called_into_predicate([Values(0, 1, 2, 3, 5, 8)] int calledNumberOfTimes)
        {
            for (int i = 0; i < calledNumberOfTimes; i++)
            {
                this.FakedFoo.Baz();
            }

            this.asserter.WasCalled(x => x.Baz(), x => x == calledNumberOfTimes);
        }

        [Test]
        [SetCulture("en-US")]
        public void WasCalled_for_void_call_should_have_correct_exception_message()
        {
            var exception = Catch<ExpectationException>(() =>
                this.asserter.WasCalled(x => x.Bar(), times => times > 10));

            Assert.That(exception.Message,
                Text.StartsWith("Expected to find call FakeItEasy.Tests.IFoo.Bar() the number of times specified by the predicate 'times => (times > 10)' but found it 0 times among the calls:"));
        }

        [Test]
        public void WasCalled_for_functions_with_repeat_predicate_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                this.asserter.WasCalled(x => x.Baz(), x => x > 10));
        }

        [Test]
        public void WasCalled_for_functions_should_not_throw_when_method_was_called_and_predicate_returns_true()
        {
            this.FakedFoo.Baz();

            this.asserter.WasCalled(x => x.Baz(), x => true);
        }

        [Test]
        public void WasCalled_for_functions_should_throw_when_predicate_returns_false()
        {
            this.FakedFoo.Baz();

            Assert.Throws<ExpectationException>(() =>
                this.asserter.WasCalled(x => x.Baz(), x => false));
        }

        [Test]
        public void WasCalled_for_functions_should_pass_the_number_of_times_a_method_was_called_into_predicate([Values(0, 1, 2, 3, 5, 8)] int calledNumberOfTimes)
        {
            for (int i = 0; i < calledNumberOfTimes; i++)
            {
                this.FakedFoo.Baz();
            }

            this.asserter.WasCalled(x => x.Baz(), x => x == calledNumberOfTimes);
        }

        [Test]
        [SetCulture("en-US")]
        public void WasCalled_for_functions_should_have_correct_exception_message()
        {
            var exception = Catch<ExpectationException>(() =>
                this.asserter.WasCalled(x => x.Baz(), times => times > 10));

            Assert.That(exception.Message,
                Text.StartsWith("Expected to find call FakeItEasy.Tests.IFoo.Baz() the number of times specified by the predicate 'times => (times > 10)' but found it 0 times among the calls:"));
        }

        private TException Catch<TException>(Action call) where TException : Exception
        {
            try
            {
                call();
            }
            catch (TException ex)
            {
                return ex;
            }

            throw new NotSupportedException();
        }
    }
}