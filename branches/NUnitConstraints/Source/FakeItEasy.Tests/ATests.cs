using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Api;
using System.Diagnostics;
using FakeItEasy.Api;
using FakeItEasy.SelfInitializedFakes;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using FakeItEasy.Expressions;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ATests
        : ConfigurableServiceLocatorTestBase
    {
        private FakeObjectFactory factory;
        private FakeObject fake;
        
        protected override void OnSetUp()
        {
            this.fake = new FakeObject();

            this.StubResolve<FakeObject.Factory>(() => this.fake);

            this.factory = A.Fake(new FakeObjectFactory(ServiceLocator.Current.Resolve<IFakeObjectContainer>(), ServiceLocator.Current.Resolve<IProxyGenerator>(), () => this.fake));

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

            Fake.Assert(this.factory).WasCalled(x => x.CreateFake(typeof(IFoo), null, false));
        }

        [Test]
        public void Generic_Fake_with_no_arguments_should_return_fake_from_factory()
        {
            var foo = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                Configure.Fake(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, false))
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

            Configure.Fake(wrapped).CallsTo(x => x.Baz()).Returns(10);

            Assert.That(wrapper.Baz(), Is.EqualTo(10));
        }

        [Test]
        public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        {
            var wrapped = A.Fake<IFoo>();
            var wrapper = A.Fake(wrapped);

            Configure.Fake(wrapped).CallsTo(x => x.Biz()).Returns("wrapped");
            Configure.Fake(wrapper).CallsTo(x => x.Biz()).Returns("wrapper");

            Assert.That(wrapper.Biz(), Is.EqualTo("wrapper"));
        }

        [Test]
        public void Fake_with_wrapped_instance_should_add_WrappedFakeObjectRule_to_fake_object()
        {
            var wrapped = A.Fake<IFoo>();

            var foo = this.CreateFakeObject<IFoo>();

            using (Fake.CreateScope())
            {
                Configure.Fake(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(IFoo), null, false))
                    .Returns(foo.Object);

                A.Fake(wrapped);

                Assert.That(foo.Rules.ToArray(), Has.Some.InstanceOf<WrappedObjectRule>());
            }
        }

        [Test]
        public void Generic_Fake_with_constructor_call_expression_should_pass_values_from_constructor_expression_to_fake_factory()
        {
            var serviceProvider = A.Fake<IServiceProvider>();

            A.Fake<Foo>(() => new Foo(serviceProvider));

            Fake.Assert(this.factory)
                .WasCalled(x => x.CreateFake(typeof(Foo), A<IEnumerable<object>>.That.IsThisSequence(new object[] {serviceProvider}).Argument, false));
        }

        [Test]
        public void Generic_Fake_with_constructor_call_should_return_object_from_factory()
        {
            var foo = A.Fake<Foo>();

            Configure.Fake(this.factory)
                .CallsTo(x => x.CreateFake(typeof(Foo), A<IEnumerable<object>>.Ignored.Argument, false))
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
                Configure.Fake(this.factory)
                    .CallsTo(x => x.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.Ignored.Argument, false))
                    .Returns(foo);

                var returned = A.Fake<AbstractTypeWithNoDefaultConstructor>(constructorArguments);

                Assert.That(returned, Is.SameAs(foo));
            }
        }

        [Test]
        public void Generic_Fake_with_arguments_for_constructor_should_pass_arguments_to_factory()
        {
            var constructorArguments = new object[] { A.Fake<IServiceProvider>() };
            IServiceProvider p = (IServiceProvider)constructorArguments[0];
            using (Fake.CreateScope())
            {
                A.Fake<AbstractTypeWithNoDefaultConstructor>(constructorArguments);

                Fake.Assert(this.factory)
                    .WasCalled(x => x.CreateFake(typeof(AbstractTypeWithNoDefaultConstructor), A<IEnumerable<object>>.That.IsThisSequence(constructorArguments).Argument, false));
            }
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFoo>(new object[] { "foo", 1 }));
        }

        [Test]
        public void Fake_with_wrapped_instance_and_recorder_should_add_SelfInitializationRule_to_fake_object()
        {
            var recorder = A.Fake<ISelfInitializingFakeRecorder>();
            var wrapped = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                this.StubResolve<FakeObject.Factory>(() => fake);
                
                var wrapper = A.Fake<IFoo>(wrapped, recorder);

                Assert.That(this.fake.Rules.First(), Is.InstanceOf<SelfInitializationRule>());
            }
        }

        [Test]
        public void Fake_with_wrapped_instance_and_recorder_should_return_fake_object_from_factory()
        {
            var recorder = A.Fake<ISelfInitializingFakeRecorder>();
            var wrapped = A.Fake<IFoo>();

            using (Fake.CreateScope())
            {
                this.StubResolve<FakeObject.Factory>(() => fake);

                var wrapper = A.Fake<IFoo>(wrapped, recorder);

                Assert.That(FakeItEasy.Fake.GetFakeObject(wrapper), Is.SameAs(this.fake));
            }
        }

        [Test]
        public void Fake_with_wrapped_instance_and_recorder_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                A.Fake<IFoo>(A.Fake<IFoo>(), A.Fake<ISelfInitializingFakeRecorder>()));
        }

        [Test]
        public void Dummy_should_return_fake_from_factory()
        {
            Configure.Fake(this.factory)
                .CallsTo(x => x.CreateFake(typeof(string), A<IEnumerable<object>>.Ignored.Argument, true))
                .Returns("return this");

            var result = A.Dummy<string>();

            Assert.That(result, Is.EqualTo("return this"));
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

        private FakeObject CreateFakeObject<T>()
        {
            return Fake.GetFakeObject(ServiceLocator.Current.Resolve<FakeObjectFactory>().CreateFake(typeof(T), null, false));
        }

    }

    [TestFixture]
    public class CallToTests
        : ConfigurableServiceLocatorTestBase
    {
        IFakeConfigurationManager configurationManager;

        protected override void OnSetUp()
        {
            this.configurationManager = A.Fake<IFakeConfigurationManager>(ServiceLocator.Current.Resolve<IFakeConfigurationManager>());
            this.StubResolve<IFakeConfigurationManager>(this.configurationManager);
        }
        
        [Test]
        public void CallTo_with_void_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Action> callSpecification = () => foo.Bar();

            var configuration = A.Fake<IVoidArgumentValidationConfiguration>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }

        [Test]
        public void CallTo_with_function_call_should_return_configuration_from_configuration_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            Expression<Func<int>> callSpecification = () => foo.Baz();

            var configuration = A.Fake<IReturnValueArgumentValidationConfiguration<int>>();
            A.CallTo(() => this.configurationManager.CallTo(callSpecification)).Returns(configuration);

            // Act
            var result = A.CallTo(callSpecification);

            // Assert
            Assert.That(result, Is.SameAs(configuration));
        }
    }

    [TestFixture]
    public class AArgumentValidationsTests
    {
        [Test]
        public void That_should_return_root_validations()
        {
            // Arrange

            // Act
            var validations = A<string>.That;

            // Assert
            Assert.That(validations, Is.InstanceOf<RootValidations<string>>());
        }

        [Test]
        public void Ignored_should_return_validator_that_passes_any_argument(
            [Values(null, "", "hello world", "foo")] string argument)
        {
            // Arrange

            // Act
            var isValid = A<string>.Ignored.IsValid(argument);

            // Assert
            Assert.That(isValid, Is.True);
        }

        [Test]
        public void Ignored_should_return_validator_with_correct_description()
        {
            // Arrange
            
            // Act
            var description = A<string>.Ignored.ToString();

            // Assert
            Assert.That(description, Is.EqualTo("<Ignored>"));
        }

        [Test]
        public void Ignored_should_return_validator_with_root_validations_set()
        {
            // Arrange

            // Act
            var validator = A<string>.Ignored;

            // Assert
            Assert.That(validator.Scope, Is.InstanceOf<RootValidations<string>>());
        }
    }
}