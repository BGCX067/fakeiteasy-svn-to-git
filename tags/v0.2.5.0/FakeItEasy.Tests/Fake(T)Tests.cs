using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using FakeItEasy.Tests.FakeConstraints;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTTests
    {
        [Test]
        public void Constructor_creates_fake_object_and_sets_it_to_the_FakedObject_property()
        {
            var fake = new Fake<IFoo>();

            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedObject>());
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_creates_fake_object_and_sets_it_to_the_FakedObject_property()
        {
            var fake = new Fake<Foo>(() => new Foo(A.Fake<IServiceProvider>()));
            
            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedObject>());
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_passes_constructor_argument_to_constructor_of_faked_object()
        {
            var constructorArgument = A.Fake<IServiceProvider>();
            var fake = new Fake<Foo>(() => new Foo(constructorArgument));

            Assert.That(fake.FakedObject.ServiceProvider, Is.SameAs(constructorArgument));
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(() => new Foo(A.Fake<IServiceProvider>())));
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_create_fake_and_set_it_to_the_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            var fake = new Fake<IFoo>(foo);

            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedObject>());
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_create_fake_that_is_wrapper()
        {
            var fake = new Fake<IFoo>(A.Fake<IFoo>());

            Assert.That(fake.FakedObject, new WrappingFakeConstraint());
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<IFoo>(A.Fake<IFoo>()));
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(new object[] { A.Fake<IServiceProvider>() }));
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_set_fake_to_FakedObject_property()
        {
            var fake = new Fake<AbstractTypeWithNoDefaultConstructor>(new object[] { A.Fake<IFoo>() });

            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedObject>());
        }

        [Test]
        public void Constructor_that_takes_arguments_for_constructor_should_throw_when_faked_type_has_accessible_constructor()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Fake<Foo>(new object[] { A.Fake<IServiceProvider>() }));
        }

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            protected AbstractTypeWithNoDefaultConstructor(IFoo foo)
            { 
            
            }
        }

        [Test]
        public void Assert_gets_a_fake_assertions_object_for_the_faked_object()
        {
            var fake = new Fake<IFoo>();

            var assertions = fake.Assert() as FakeAsserter<IFoo>;
            
            Assert.That(assertions.Fake.Object, Is.SameAs(fake.FakedObject));
        }

        [Test]
        public void RecordedCalls_returns_a_call_collection_for_the_faked_obejct()
        {
            var fake = new Fake<IFoo>();
            
            fake.FakedObject.Bar();

            var calls = fake.RecordedCalls;
            
            Assert.That(calls.FakedObject, Is.SameAs(fake.FakedObject));
        }

        [Test]
        public void Calls_to_returns_fake_configuration_for_the_faked_object_when_void_call_is_specified()
        {
            var fake = new Fake<IFoo>();

            IVoidConfiguration<IFoo> voidConfiguration  = fake.CallsTo(x => x.Bar()) as FakeConfiguration<IFoo>;
            var configuration = voidConfiguration as FakeConfiguration<IFoo>;

            Assert.That(configuration.Fake.Object, Is.SameAs(fake.FakedObject));
        }

        [Test]
        public void Calls_to_returns_fake_configuraion_for_the_faked_object_when_function_call_is_specified()
        {
            var fake = new Fake<IFoo>();

            IReturnValueConfiguration<IFoo, int> returnValueConfiguration = fake.CallsTo(x => x.Baz());
            var configuration = returnValueConfiguration as FakeConfiguration<IFoo>.ReturnValueConfiguration<int>;

            Assert.That(configuration.ParentConfiguration.Fake.Object, Is.SameAs(fake.FakedObject));
        }

        [Test]
        public void AnyCall_returns_fake_configuration_for_the_faked_object()
        {
            var fake = new Fake<IFoo>();

            IVoidConfiguration<IFoo> voidConfiguration = fake.AnyCall();
            var configuration = voidConfiguration as FakeConfiguration<IFoo>;

            Assert.That(configuration.Fake.Object, Is.SameAs(fake.FakedObject));
        }
    }
}
