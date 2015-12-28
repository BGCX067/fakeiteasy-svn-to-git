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
using FakeItEasy.Configuration;

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

        [Test, Explicit]
        public void ErrorMessage_when_type_can_not_be_faked_should_specify_non_resolvable_constructor_arguments()
        {
            using (Fake.CreateScope())
            {
                var thrown = Assert.Throws<ArgumentException>(() =>
                    A.Fake<NonResolvableType>());

                Assert.That(thrown.Message, Is.EqualTo(@"

  Can not create fake of the type 'FakeItEasy.IntegrationTests.GeneralTests+NonResolvableType'.

  1. The type is not registered in the current IFakeObjectContainer.
  2. The current IProxyGenerator failed to generate a proxy for the following reason:
     
     The type has no default constructor and none of the available constructors listed below can be resolved:

     Constructor: (FakeItEasy.Tests.IFoo, System.String*)
     Constructor: (System.String*)

     * The types marked with with a star (*) can not be faked. Register these types in the current
     IFakeObjectContainer in order to generate this fake.

"));
            }
            
        }

        public class NonResolvableType
        {
            public NonResolvableType(IFoo foo, string bar)
            {

            }

            public NonResolvableType(string bar)
            {

            }
        }

        [Test]
        public void ErrorMessage_when_configuring_void_call_that_can_not_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualVoidMethod("", 1)).DoesNothing());

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_function_call_that_can_not_be_configured_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                A.CallTo(() => fake.NonVirtualFunction("", 1)).Returns(10));

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_void_call_that_can_not_be_configured_through_old_api_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                Configure.Fake(fake).CallsTo(x => x.NonVirtualVoidMethod("", 1)).DoesNothing());

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        [Test]
        public void ErrorMessage_when_configuring_function_call_that_can_not_be_configured_through_old_api_should_be_correct()
        {
            // Arrange
            var fake = A.Fake<TypeWithNonConfigurableMethod>();

            // Act

            // Assert
            var thrown = Assert.Throws<FakeConfigurationException>(() =>
                Configure.Fake(fake).CallsTo(x => x.NonVirtualFunction("", 1)).Returns(10));

            Assert.That(thrown.Message, Is.EqualTo("The specified method can not be configured since it can not be intercepted by the current IProxyGenerator."));
        }

        public class TypeWithNonConfigurableMethod
        {
            public void NonVirtualVoidMethod(string argument, int otherArgument)
            {
                
            }

            public int NonVirtualFunction(string argument, int otherArgument)
            {
                return 1;
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
