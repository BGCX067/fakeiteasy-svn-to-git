using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using System.Linq.Expressions;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTests
    {
        [Test]
        public void Configure_should_return_fake_configuration_when_called_on_fake_object()
        {
            var foo = A.Fake<IFoo>();

            var configuration = (FakeConfiguration<IFoo>)Fake.Configure(foo);

            Assert.That(configuration.Fake, Is.EqualTo(Fake.GetFakeObject(foo)));
        }

        [Test]
        public void Configure_should_throw_when_FakedObject_is_not_a_faked_object()
        {
            Assert.Throws<ArgumentException>(() =>
                Fake.Configure(""));
        }

        [Test]
        public void Configure_should_throw_when_fakedObject_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Fake.Configure((IFoo)null));
        }

        [Test]
        public void GetCalls_gets_all_calls_to_fake_object()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Baz();
            foo.Biz();

            var calls = Fake.GetCalls(foo);
            var namesOfCalledMethods = calls.Select(x => x.Method.Name).ToArray();

            Assert.That(namesOfCalledMethods, Is.EquivalentTo(new[] { "Bar", "Baz", "Biz" }));            
        }

        [Test]
        public void GetCalls_is_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => Fake.GetCalls(A.Fake<IFoo>()));
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Fake.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(Fake.ReferenceEquals(s, s), Is.True);
        }
    }
}
