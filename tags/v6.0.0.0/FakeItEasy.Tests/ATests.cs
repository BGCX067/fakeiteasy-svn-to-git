using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Extensibility;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ATests
        : ConfigurableServiceLocatorTestBase
    {
        private FakeObjectFactory factory;

        protected override void OnSetUp()
        {
            this.factory = A.Fake(ServiceLocator.Current.Resolve<FakeObjectFactory>());

            this.StubResolve<FakeObjectFactory>(this.factory);
        }

        [Test]
        public void Fake_with_constructor_call_throws_when_passed_in_expression_is_not_constructor_call()
        {
            Assert.Throws<ArgumentException>(() =>
                A.Fake<IFoo>(() => CreateFoo()));
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(A.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(A.ReferenceEquals(s, s), Is.True);
        }

        [Test]
        public void Generic_Fake_with_no_arguments_should_call_fake_object_factory_with_correct_arguments()
        {
            A.Fake<IFoo>();

            Fake.Assert(this.factory).WasCalled(x => x.CreateFake(typeof(IFoo), null, true));
        }

        [Test]
        public void Generic_Fake_with_no_arguments_should_return_fake_from_factory()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, true))
                    .Returns(foo);

                var returned = A.Fake<IFoo>();

                Assert.That(returned, Is.SameAs(foo));
            }
        }

        [Test]
        public void Fake_called_with_instance_returns_wrapping_fake_that_delegates_to_wrapped_object()
        {            
            var wrapped = A.Fake<IFoo>();
            var wrapper = A.Fake(wrapped);

            Fake.Configure(wrapped).CallsTo(x => x.Baz()).Returns(10);

            Assert.That(wrapper.Baz(), Is.EqualTo(10));
        }

        [Test]
        public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        {
            var wrapped = A.Fake<IFoo>();
            var wrapper = A.Fake(wrapped);

            Fake.Configure(wrapped).CallsTo(x => x.Biz()).Returns("wrapped");
            Fake.Configure(wrapper).CallsTo(x => x.Biz()).Returns("wrapper");

            Assert.That(wrapper.Biz(), Is.EqualTo("wrapper"));
        }

        [Test]
        public void Fake_with_wrapped_instance_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            var wrapped = A.Fake<IFoo>();

            var foo = this.CreateFakeObject<IFoo>();

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, true))
                    .Returns(foo.Object);

                A.Fake(wrapped);

                Assert.That(foo.Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
            }
        }

        private FakeObject CreateFakeObject<T>()
        {
            return Fake.GetFakeObject(ServiceLocator.Current.Resolve<FakeObjectFactory>().CreateFake(typeof(T), null, false));
        }

        [Test]
        public void Generic_Fake_with_constructor_call_expression_should_pass_values_from_constructor_expression_to_fake_factory()
        {
            var serviceProvider = A.Fake<IServiceProvider>();

            A.Fake<Foo>(() => new Foo(serviceProvider));

            Fake.Assert(this.factory)
                .WasCalled(x => x.CreateFake(typeof(Foo), Argument.Is.SameSequence(new object[] {serviceProvider}), true));
        }

        [Test]
        public void Generic_Fake_with_constructor_call_should_return_object_from_factory()
        {
            var foo = A.Fake<Foo>();

            Fake.Configure(this.factory)
                .CallsTo(x => x.CreateFake(typeof(Foo), Argument.Is.Any<IEnumerable<object>>(), true))
                .Returns(foo);

            var returned = A.Fake<Foo>(() => new Foo(A.Fake<IServiceProvider>()));

            Assert.That(returned, Is.SameAs(foo));
        }

        [Test]
        public void Fake_with_wrapper_should_be_guarded_correctly()
        {
            NullGuardedConstraint.Assert(() => A.Fake(A.Fake<IFoo>()));
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_throw_if_the_fake_type_is_not_abstract()
        {
            Assert.Throws<InvalidOperationException>(() =>
                A.Fake<Foo>(new object[] { "bar" }));
        }

        [Test]
        public void Generic_Fake_with_arguments_for_constructor_should_return_fake_from_factory()
        {
            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
            var foo = A.Fake<AbstractTypeWithNoDefaultConstructor>(constructorArguments);

            using (Fake.CreateScope())
            {
                Fake.Configure(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), Argument.Is.Any<IEnumerable<object>>(), true))
                    .Returns(foo);

                var returned = A.Fake<AbstractTypeWithNoDefaultConstructor>(constructorArguments);

                Assert.That(returned, Is.SameAs(foo));
            }
        }

        [Test]
        public void Generic_Fake_with_arguments_for_constructor_should_pass_arguments_to_factory()
        {
            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };

            using (Fake.CreateScope())
            {
                A.Fake<AbstractTypeWithNoDefaultConstructor>(constructorArguments);

                Fake.Assert(this.factory)
                    .WasCalled(x => x.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), Argument.Is.SameSequence(constructorArguments), true));
            }
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFoo>(new object[] { "foo", 1 }));
        }

        private static IFoo CreateFoo()
        {
            return null;
        }

        public abstract class AbstractTypeWithNoDefaultConstructor
        {
            protected AbstractTypeWithNoDefaultConstructor(IServiceProvider serviceProvider)
            { 
            
            }
        }
    }
}