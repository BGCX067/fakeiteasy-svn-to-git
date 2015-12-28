using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.Configuration;
using Legend.Fakes.Api;
using NUnit.Framework.Constraints;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Reflection;
using Legend.Fakes.Assertion;

namespace Legend.Fakes.Tests.Assertion
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
    }
}