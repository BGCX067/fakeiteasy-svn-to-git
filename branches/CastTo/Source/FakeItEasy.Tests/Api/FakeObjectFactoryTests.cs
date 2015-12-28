using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.ExtensionSyntax;
using FakeItEasy.Api;
using FakeItEasy.Configuration;
using FakeItEasy.Assertion;
using FakeItEasy.Tests.TestHelpers;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class FakeObjectFactoryTests
    {
        IFakeObjectContainer container;
        IProxyGenerator proxyGenerator;
        FakeObject.Factory fakeObjectFactory;
        FakeObject fakeObject;

        [SetUp]
        public void SetUp()
        {
            this.container = A.Fake<IFakeObjectContainer>();
            this.proxyGenerator = A.Fake<IProxyGenerator>();
            this.fakeObject = A.Fake<FakeObject>();
            this.fakeObjectFactory = () => this.fakeObject;
        }

        private FakeObjectFactory CreateFactory()
        {
            return new FakeObjectFactory(this.container, this.proxyGenerator, this.fakeObjectFactory);
        }

        [Test]
        public void CreateFake_should_return_fake_from_container_when_non_proxyied_objects_are_allowed_and_container_contains_type()
        {
            var factory = this.CreateFactory();

            object result = null;
            A.CallTo(() => this.container.TryCreateFakeObject(typeof(int), out result)).Returns(true).AssignsOutAndRefParameters(1);

            Assert.That(factory.CreateFake(typeof(int), null, true), Is.EqualTo(1));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_contains_type_but_non_proxied_objects_are_not_allowed()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out)).Returns(true).AssignsOutAndRefParameters(A.Fake<IFoo>());
            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            Assert.That(factory.CreateFake(typeof(IFoo), null, false), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_container_does_not_contain_type()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            Assert.That(factory.CreateFake(typeof(IFoo), null, true), Is.SameAs(returned.Proxy));
        }

        [Test]
        public void CreateFake_should_set_generated_proxy_to_fake_object()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("foo").Argument)).Returns(returned);

            factory.CreateFake(typeof(IFoo), new object[] { "foo" }, false);

            A.CallTo(() => this.fakeObject.SetProxy(returned)).Assert(Happened.Once);
        }

        [Test]
        public void CreateFake_should_throw_exception_when_fake_cant_be_resolved_from_container_or_generated()
        {
            var factory = this.CreateFactory();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container)).Returns(new FailedProxyResult(typeof(IFoo)));

            var thrown = Assert.Throws<ArgumentException>(() =>
                factory.CreateFake(typeof(IFoo), null, true));

            Assert.That(thrown.Message, Is.EqualTo("Can not create fake of the type 'FakeItEasy.Tests.IFoo', it's not registered in the current container and the current IProxyGenerator can not generate the fake."));
        }

        [Test]
        public void CreateFake_should_pass_created_proxy_to_ConfigureFake_on_container()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, this.container)).Returns(returned);

            factory.CreateFake(typeof(IFoo), null, true);

            A.CallTo(() => this.container.ConfigureFake(typeof(IFoo), returned.Proxy)).Assert(Happened.Once);
        }

        [Test]
        public void CreateFake_should_return_fake_from_proxy_generator_when_arguments_for_constructor_is_specified_even_though_non_proxied_fakes_are_accepted()
        {
            var factory = this.CreateFactory();

            var returned = new TestableProxyResult(typeof(IFoo), (IFakedProxy)A.Fake<IFoo>());

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(IFoo), A<FakeObject>.Ignored, A<IEnumerable<object>>.That.IsThisSequence("argument for constructor").Argument)).Returns(returned);

            factory.CreateFake(typeof(IFoo), new object[] { "argument for constructor" }, true);

            A.CallTo(() => this.container.TryCreateFakeObject(typeof(IFoo), out Null<object>.Out)).Assert(Happened.Never);
        }

        [Test]
        public void CreateFake_should_throw_when_proxy_generator_can_not_generate_fake_with_arguments_for_constructor()
        {
            // Arrange
            var factory = this.CreateFactory();

            A.CallTo(() => this.proxyGenerator.GenerateProxy(typeof(string), A<FakeObject>.Ignored, A<IEnumerable<object>>.Ignored.Argument)).Returns(new FailedProxyResult(typeof(string)));

            // Act
            
            // Assert
            Assert.Throws<ArgumentException>(() =>
                factory.CreateFake(typeof(string), new object[] { }, false));
        }

        [Test]
        [SetCulture("en-US")]
        public void CastTo_should_fail_when_proxy_generator_does_not_support_adding_of_interfaces()
        {
            // Arrange
            var factory = this.CreateFactory();
            var foo = A.Fake<IFoo>();

            // Act
            
            // Assert
            var thrown = Assert.Throws<NotSupportedException>(() =>
                factory.CastTo(typeof(IBar), foo));

            Assert.That(thrown, Has.Message.EqualTo("The current IProxyGenerator does not support multiple interfaces per fake."));
        }

        [Test]
        public void CastTo_should_call_add_interface_to_proxy_on_proxy_generator()
        {
            // Arrange
            this.ConfigureProxyGeneratorToSupportMultipleInterfaces();
            var factory = this.CreateFactory();
            var foo = A.Fake<IFoo>();

            // Act
            factory.CastTo(typeof(IBar), foo);

            // Assert
            var generator = this.proxyGenerator as ProxyGeneratorWithMultiSupport;
            Assert.That(generator.ProxyArgument, Is.EqualTo(foo));
            Assert.That(generator.InterfaceTypeArgument, Is.EqualTo(typeof(IBar)));
        }

        [Test]
        public void CastTo_should_not_call_proxy_generator_if_the_proxy_already_supports_the_specified_interface_type()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            this.ConfigureProxyGeneratorToSupportMultipleInterfaces();

            var factory = this.CreateFactory();

            // Act
            factory.CastTo(typeof(IFoo), foo);

            // Assert
            var generator = this.proxyGenerator as ProxyGeneratorWithMultiSupport;
            Assert.That(generator.InterfaceTypeArgument, Is.Null);
        }

        [Test]
        public void CastTo_should_throw_when_interfaceType_parameter_is_not_an_interface()
        {
            // Arrange
            var factory = this.CreateFactory();

            // Act
            
            // Assert
            Assert.That(() => factory.CastTo(typeof(Foo), A.Fake<IFoo>()),
                Throws.TypeOf<ArgumentException>().With.Message.StartsWith("The specified type is not an interface type."));
        }

        private void ConfigureProxyGeneratorToSupportMultipleInterfaces()
        {
            this.proxyGenerator = new ProxyGeneratorWithMultiSupport { ProxyGeneratorFake = this.proxyGenerator };
        }

        private class ProxyGeneratorWithMultiSupport
            : IProxyGeneratorWithMultipleInterfaceSupport, IFakedProxy
        {
            public IProxyGenerator ProxyGeneratorFake;
            public Type InterfaceTypeArgument;
            public object ProxyArgument;


            public void AddInterfaceToProxy(Type interfaceType, object proxy)
            {
                this.InterfaceTypeArgument = interfaceType;
                this.ProxyArgument = proxy;
            }

            public ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor)
            {
                return this.ProxyGeneratorFake.GenerateProxy(typeToProxy, fakeObject, argumentsForConstructor);
            }

            public ProxyResult GenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container)
            {
                return this.ProxyGeneratorFake.GenerateProxy(typeToProxy, fakeObject, container);
            }

            public bool MemberCanBeIntercepted(System.Reflection.MemberInfo member)
            {
                return this.ProxyGeneratorFake.MemberCanBeIntercepted(member);
            }

            public FakeObject FakeObject
            {
                get { return Fake.GetFakeObject(this.ProxyGeneratorFake); }
            }
        }


        private class FailedProxyResult
            : ProxyResult
        {
            public FailedProxyResult(Type proxiedType)
                : base(proxiedType)
            {
                this.ProxyWasSuccessfullyCreated = false;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
        }

        private class SuccessfulProxyResult
            : ProxyResult
        {
            public SuccessfulProxyResult(Type proxiedType)
                : base(proxiedType)
            {
                this.ProxyWasSuccessfullyCreated = true;
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
        }
    }
}
