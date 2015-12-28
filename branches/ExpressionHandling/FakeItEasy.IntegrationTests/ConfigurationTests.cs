using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Tests;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.VisualBasic;

namespace FakeItEasy.IntegrationTests
{
    [TestFixture]
    public class ConfigurationTests
    {
        [Test]
        public void Faked_object_configured_to_call_back_performs_callback_when_called()
        {
            var foo = A.Fake<IFoo>();

            bool wasCalled = false;

            foo.Configure().CallsTo(x => x.Bar()).Invokes(x => wasCalled = true);
            
            foo.Bar();

            Assert.That(wasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_perform_several_call_backs_and_return_value_does_all()
        {
            var foo = A.Fake<IFoo>();
            
            bool firstWasCalled = false;
            bool secondWasCalled = false;

            foo.Configure()
                .CallsTo(x => x.Baz())
                .Invokes(x => firstWasCalled = true)
                .Invokes(x => secondWasCalled = true)
                .Returns(10);

            var result = foo.Baz();

            Assert.That(firstWasCalled);
            Assert.That(secondWasCalled);
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_call_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.False);

            fake.Configure().CallsTo(x => x.DoSomething()).CallsBaseMethod();
            fake.DoSomething();

            Assert.That(fake.WasCalled, Is.True);
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_should_return_value_from_base_method()
        {
            var fake = A.Fake<BaseClass>();

            fake.Configure().CallsTo(x => x.ReturnSomething()).CallsBaseMethod();
            
            Assert.That(fake.ReturnSomething(), Is.EqualTo(10));
        }

        [Test]
        public void Faked_object_configured_to_call_base_method_invokes_configured_invokations_also()
        {
            var fake = A.Fake<BaseClass>();

            bool wasCalled = false;

            fake.Configure().CallsTo(x => x.ReturnSomething()).Invokes(x => wasCalled = true).CallsBaseMethod();

            fake.ReturnSomething();

            Assert.That(wasCalled, Is.True);
        }

        public class BaseClass
        {
            public bool WasCalled;

            public virtual void DoSomething()
            {
                WasCalled = true;
            }

            public virtual int ReturnSomething()
            {
                return 10;
            }
        }
    }
}
