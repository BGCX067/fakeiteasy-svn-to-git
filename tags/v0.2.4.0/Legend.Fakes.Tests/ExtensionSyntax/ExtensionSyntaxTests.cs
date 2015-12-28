using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Legend.Fakes.ExtensionSyntax;
using Legend.Fakes.Configuration;

namespace Legend.Fakes.Tests.ExtensionSyntax
{
    [TestFixture]
    public class ExtensionSyntaxTests
    {
        [Test]
        public void Configure_should_return_fake_configuration_when_called_on_fake_object()
        {
            var foo = A.Fake<IFoo>();

            var configuration = (FakeConfiguration<IFoo>)foo.Configure();

            Assert.That(configuration.Fake, Is.EqualTo(Fake.GetFakeObject(foo)));
        }

        [Test]
        public void Configure_should_throw_when_FakedObject_is_not_a_faked_object()
        {
            Assert.Throws<ArgumentException>(() =>
                "".Configure());
        }

        [Test]
        public void Configure_should_throw_when_fakedObject_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
                Legend.Fakes.ExtensionSyntax.Syntax.Configure((IFoo)null));
        }
    }
}
