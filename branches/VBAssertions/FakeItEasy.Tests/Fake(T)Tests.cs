﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using FakeItEasy.Tests.FakeConstraints;
using FakeItEasy.Extensibility;
using System.Reflection;
using FakeItEasy.Expressions;
using System.Linq.Expressions;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTTests
        : ConfigurableServiceLocatorTestBase
    {
        private FakeObjectFactory factory;

        protected override void OnSetUp()
        {
            this.factory = A.Fake(new FakeObjectFactory(A.Fake<IFakeObjectContainer>()));

            FakeObjectFactory.Creator creator = () => this.factory;

            this.StubResolve<FakeObjectFactory.Creator>(creator);
        }
        
        [Test]
        public void Constructor_sets_fake_object_returned_from_factory_to_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, false)).Returns(foo);

                var fake = new Fake<IFoo>();

                Assert.That(fake.FakedObject, Is.SameAs(foo));
            }
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_sets_fake_object_returned_from_factory_to_FakedObject_property()
        {
            var foo = A.Fake<Foo>();
            var serviceProviderArgument = A.Fake<IServiceProvider>();

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(Foo), Its.SameSequence(new object[] { serviceProviderArgument }), false))
                    .Returns(foo);

                var fake = new Fake<Foo>(() => new Foo(serviceProviderArgument));

                Assert.That(fake.FakedObject, Is.SameAs(foo));
            }
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new Fake<Foo>(() => new Foo(A.Fake<IServiceProvider>())));
        }

        [Test]
        public void Constructor_that_takes_constructor_expression_should_throw_if_the_specified_expression_is_not_a_constructor_call()
        {
            Assert.Throws<ArgumentException>(() =>
                new Fake<Foo>(() => CreateFoo()));
        }

        private static Foo CreateFoo()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void Constructor_that_takes_wrapped_instance_should_create_fake_and_set_it_to_the_FakedObject_property()
        {
            var foo = A.Fake<IFoo>();

            var fake = new Fake<IFoo>(foo);

            Assert.That(fake.FakedObject, Is.InstanceOfType<IFakedProxy>());
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
        public void Constructor_that_takes_arguments_for_constructor_should_set_fake_returned_from_factory_to_FakedObject_property()
        {
            var argumentsForConstructor = new object[] { A.Fake<IFoo>() };
            var fakeReturnedFromFactory = A.Fake<AbstractTypeWithNoDefaultConstructor>(argumentsForConstructor);

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), Its.SameSequence(argumentsForConstructor), false))
                    .Returns(fakeReturnedFromFactory);

                var fake = new Fake<AbstractTypeWithNoDefaultConstructor>(argumentsForConstructor);

                Assert.That(fake.FakedObject, Is.SameAs(fakeReturnedFromFactory));
            }
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
        public void AssertWasCalled_with_void_call_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();
            
            Expression<Action<IFoo>> callSpecification = x => x.Bar();

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                Fake.Configure(factory)
                    .CallsTo(x => x.CreateAsserter<IFoo>(fakeObject))
                    .Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification));
        }

        [Test]
        public void AssertWasCalled_with_void_call_and_repeat_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Action<IFoo>> callSpecification = x => x.Bar();
            Expression<Func<int, bool>> repeat = x => true;

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                Fake.Configure(factory)
                    .CallsTo(x => x.CreateAsserter<IFoo>(fakeObject))
                    .Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification, repeat);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification, repeat));
        }

        [Test]
        public void AssertWasCalled_with_function_call_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);
            
            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();
            
            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                Fake.Configure(factory)
                    .CallsTo(x => x.CreateAsserter<IFoo>(fakeObject))
                    .Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification));
        }

        [Test]
        public void AssertWasCalled_with_function_call_and_repeat_calls_WasCalled_on_assertions_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factoryAssertions = A.Fake<IFakeAssertions<IFoo>>();

            Expression<Func<IFoo, int>> callSpecification = x => x.Baz();
            Expression<Func<int, bool>> repeat = x => true;

            using (Fake.CreateScope())
            {
                var factory = this.StubResolveWithFake<IFakeAssertionsFactory>();
                Fake.Configure(factory)
                    .CallsTo(x => x.CreateAsserter<IFoo>(fakeObject))
                    .Returns(factoryAssertions);

                fake.AssertWasCalled(callSpecification, repeat);
            }

            Fake.Assert(factoryAssertions)
                .WasCalled(x => x.WasCalled(callSpecification, repeat));
        }

        [Test]
        public void RecordedCalls_returns_call_collection_from_factory()
        {
            var fake = new Fake<IFoo>();
            var fakeObject = Fake.GetFakeObject(fake.FakedObject);

            var factory = A.Fake<ICallCollectionFactory>();
            this.StubResolve<ICallCollectionFactory>(factory);

            var collectionReturnedFromFactory = A.Fake<ICallCollection<IFoo>>();
            Fake.Configure(factory)
                .CallsTo(x => x.CreateCallCollection<IFoo>(fakeObject))
                .Returns(collectionReturnedFromFactory);

            var calls = fake.RecordedCalls;

            Assert.That(calls, Is.SameAs(collectionReturnedFromFactory));
        }

        [Test]
        public void Calls_to_returns_fake_configuration_for_the_faked_object_when_void_call_is_specified()
        {
            var fake = new Fake<IFoo>();

            IVoidConfiguration<IFoo> voidConfiguration = fake.CallsTo(x => x.Bar()) as FakeConfiguration<IFoo>;
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
