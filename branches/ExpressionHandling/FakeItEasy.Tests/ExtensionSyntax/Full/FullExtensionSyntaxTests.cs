using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.ExtensionSyntax.Full;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;

namespace FakeItEasy.Tests.ExtensionSyntax.Full
{
    [TestFixture]
    public class FullExtensionSyntaxTests
    {
        [Test]
        public void CallsTo_for_return_value_methods_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.CallsTo("string", x => x.Length));
        }

        [Test]
        public void CallsTo_for_return_value_methods_should_return_fake_configuartion_for_fake()
        {
            var fake = A.Fake<IFoo>();

            var configuration = fake.CallsTo(x => x.Baz()) as FakeConfiguration<IFoo>.ReturnValueConfiguration<int>;

            Assert.That(configuration.ParentConfiguration.Fake.Object, Is.SameAs(fake));
        }

        [Test]
        public void CallsTo_for_void_methods_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.CallsTo(A.Fake<IFoo>(), x => x.Bar()));
        }

        [Test]
        public void CallsTo_for_void_methods_should_return_fake_configuration_for_fake()
        {
            var fake = A.Fake<IFoo>();

            var config = fake.CallsTo(x => x.Bar()) as FakeConfiguration<IFoo>;

            Assert.That(config.Fake.Object, Is.SameAs(fake));
        }

        [Test]
        public void Assert_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                FullExtensionSyntax.Assert(A.Fake<IFoo>()));
        }

        [Test]
        public void Assert_should_return_fake_assertion_object_for_faked_object()
        {
            var fake = A.Fake<IFoo>();

            var assertions = fake.Assert() as FakeAsserter<IFoo>;

            Assert.That(assertions.Fake.Object, Is.SameAs(fake));
        }
    }
}
