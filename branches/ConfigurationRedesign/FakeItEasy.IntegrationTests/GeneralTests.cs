using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;
using System.IO;
using System.Web.UI;
using FakeItEasy.Api;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public void Faked_object_with_fakeable_properties_should_have_fake_as_default_value()
        {
            var fake = A.Fake<ITypeWithFakeableProperties>();

            Assert.That(fake.Collection, Is.Not.Null);
            Assert.That(fake.Foo, Is.Not.Null);
        }

        [Test]
        public void Should_not_be_able_to_fake_Uri_when_no_container_is_used()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.Throws<ArgumentException>(() =>
                    A.Fake<Uri>());
            }
        }

        public interface ITypeWithFakeableProperties
        {
            IEnumerable<object> Collection { get; }
            IFoo Foo { get; }
        }
    }

    [TestFixture]
    public class DummyTests
    {
        [Test]
        public void Type_registered_in_container_should_be_returned_when_a_dummy_is_requested()
        {
            var container = new DelegateFakeObjectContainer();
            container.Register<string>(() => "dummy");

            using (Fake.CreateScope(container))
            {
                Assert.That(A.Dummy<string>(), Is.EqualTo("dummy"));
            }
        }

        [Test]
        public void Proxy_should_be_returned_when_nothing_is_registered_in_the_container_for_the_type()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.That(A.Dummy<IFoo>(), Is.InstanceOf<IFakedProxy>());
            }
        }

        [Test]
        public void Correct_exception_should_be_thrown_when_dummy_is_requested_for_non_fakeable_type_not_in_container()
        {
            using (Fake.CreateScope(new NullFakeObjectContainer()))
            {
                Assert.Throws<ArgumentException>(() =>
                    A.Dummy<int>());
            }
        }
    }
}
