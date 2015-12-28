using FakeItEasy.Api;
using FakeItEasy.DynamicProxy;
using FakeItEasy.Tests.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FakeItEasy.Extensibility;
using System.IO;

namespace FakeItEasy.Tests.DynamicProxy
{
    [TestFixture]
    public class DynamicProxyProxyGeneratorTests
    {
        FakeObject fakeObject;
        IFakeObjectContainer container;

        private IExtensibleIs Its;

        [SetUp]
        public void SetUp()
        {
            this.fakeObject = A.Fake<FakeObject>();
            this.container = A.Fake<IFakeObjectContainer>();
        }

        private DynamicProxyProxyGenerator CreateGenerator()
        {
            return new DynamicProxyProxyGenerator();
        }

        private List<Type> typesThatCanBeProxied = new List<Type>()
        {
            typeof(IFoo),
            typeof(ClassWithDefaultConstructor),
            typeof(AbstractClassWithDefaultConstructor)
        };

        private List<Type> typesThatCanNotBeProxied = new List<Type>()
        {
            typeof(int),
            typeof(AbstractClassWithHiddenConstructor),
            typeof(ClassWithHiddenConstructor),
            typeof(SealedClass)
        };

        [TestCaseSource("typesThatCanBeProxied")]
        public void TryGenerateProxy_should_return_true_when_type_can_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.TryGenerateProxy(typeOfProxy, this.fakeObject, this.container, out Null<ProxyResult>.Out);

            Assert.That(result, Is.True);
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void TryGenerateProxy_should_assign_result_to_out_parameter(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeOfProxy, this.fakeObject, this.container, out result);

            Assert.That(result.Proxy, Is.InstanceOf(typeOfProxy));
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void TryGenerateProxy_should_return_proxy_that_can_get_the_passed_in_proxy_manager(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeOfProxy, this.fakeObject, this.container, out result);

            Assert.That(result.Proxy.GetFakeObject(), Is.SameAs(this.fakeObject));

        }

        [TestCaseSource("typesThatCanNotBeProxied")]
        public void TryGenerateProxy_should_return_false_for_types_that_can_not_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            var generated = generator.TryGenerateProxy(typeOfProxy, this.fakeObject, this.container, out result);
            
            Assert.That(generated, Is.False);
        }

        [Test]
        public void TryGenerateProxy_should_throw_when_type_is_interface_but_arguments_for_constructor_is_specified()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            
            var thrown = Assert.Throws<ArgumentException>(() =>
                generator.TryGenerateProxy(typeof(IFoo), this.fakeObject, new object[] { 1, 2 }, out result));

            Assert.That(thrown.Message, Is.EqualTo("Arguments for constructor was specified when generating proxy of interface type."));
        }

        [TestCase(typeof(ISomeInterface))]
        [TestCase(typeof(SomeInterfaceImplementation))]
        [TestCase(typeof(SomeAbstractInterfaceImplementation))]
        public void TryGenerateProxy_should_generate_result_that_raises_events_when_calls_are_intercepted(Type someInterfaceType)
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(someInterfaceType, this.fakeObject, this.container, out result);

            IWritableFakeObjectCall interceptedCall = null;
            result.CallWasIntercepted += (s, e) =>
                {
                    interceptedCall = e.Call;
                };

            var someInterface = (ISomeInterface)result.Proxy;
            someInterface.Bar();

            Assert.That(interceptedCall.Method.Name, Is.EqualTo("Bar"));
        }

        [Test]
        public void TryGenerateProxy_should_return_true_when_generating_class_with_arguments_for_constructor_that_matches_constructor()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            var generated = generator.TryGenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" }, out result);

            Assert.That(generated, Is.True);
        }

        [Test]
        public void TryGenerateProxy_with_arguments_for_constructor_should_generate_proxies_raises_events_when_calls_made_to_proxy()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" }, out result);

            IWritableFakeObjectCall interceptedCall = null;
            result.CallWasIntercepted += (s, e) =>
                {
                    interceptedCall = e.Call;
                };

            var fake = result.Proxy as TypeWithConstructorThatTakesSingleString;
            fake.Bar();

            Assert.That(interceptedCall.Method.Name, Is.EqualTo("Bar"));
        }

        [Test]
        public void TryGenerateProxy_with_arguments_for_constructor_should_generate_proxies_that_can_get_proxy_manager()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" }, out result);

            Assert.That(result.Proxy.GetFakeObject(), Is.SameAs(this.fakeObject));
        }

        [Test]
        public void TryGenerateProxy_without_arguments_for_constructor_should_return_true_when_container_can_resolve_arguments_for_constructor()
        {
            Fake.Configure(this.container)
                .CallsTo(x => x.TryCreateFakeObject(typeof(string), null, out Null<object>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters("foo");

            var generator = this.CreateGenerator();
            
            ProxyResult result;
            var generated = generator.TryGenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, this.container, out result);

            Assert.That(generated, Is.True);
        }

        [Test]
        public void TryGenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_container_can_resolve_arguments_for_constructor()
        {
            Fake.Configure(this.container)
                .CallsTo(x => x.TryCreateFakeObject(typeof(string), null, out Null<object>.Out))
                .Returns(true)
                .AssignsOutAndRefParameters("foo");

            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, this.container, out result);

            Assert.That(result.Proxy, Is.InstanceOf<TypeWithConstructorThatTakesSingleString>());
        }

        [Test]
        public void TryGenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_value_type()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            var generated = generator.TryGenerateProxy(typeof(TypeThatTakesValueTypeInConstructor), this.fakeObject, this.container, out result);

            Assert.That(generated, Is.True);
        }

        [Test]
        public void TryGenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_interface_type()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            var generated = generator.TryGenerateProxy(typeof(TypeThatTakesProxyableTypeInConstructor), this.fakeObject, this.container, out result);

            Assert.That(generated, Is.True);
        }

        [Test]
        public void GeneratedProxies_should_be_serializable()
        {
            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(IFoo), this.fakeObject, this.container, out result);

            Assert.That(result, Is.BinarySerializable);
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_ToString()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(IFoo), this.fakeObject, this.container, out result);

            result.CallWasIntercepted += (s, e) =>
                {
                    wasIntercepted = true;
                };

            result.Proxy.ToString();

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_Equals()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(IFoo), this.fakeObject, this.container, out result);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
                e.Call.SetReturnValue(true);
            };

            result.Proxy.Equals(null);

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_GetHashCode()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            ProxyResult result;
            generator.TryGenerateProxy(typeof(IFoo), this.fakeObject, this.container, out result);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
                e.Call.SetReturnValue(1);
            };
            
            result.Proxy.GetHashCode();

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        public class TypeThatTakesProxyableTypeInConstructor
        {
            public TypeThatTakesProxyableTypeInConstructor(IFormattable formattable)
            { 
            
            }
        }

        public class TypeThatTakesValueTypeInConstructor
        {
            public TypeThatTakesValueTypeInConstructor(int value)
            {
            
            }
        }

        public interface ISomeInterface
        {
            void Bar();
        }

        public class SomeInterfaceImplementation
            : ISomeInterface
        {

            public virtual void Bar()
            {
                throw new NotImplementedException();
            }
        }

        public abstract class SomeAbstractInterfaceImplementation
            : ISomeInterface
        {

            public abstract void Bar();
        }

        public class TypeWithConstructorThatTakesSingleString
        {
            public TypeWithConstructorThatTakesSingleString(string s)
            { }

            public virtual void Bar()
            { 
            
            }
        }

        public abstract class AbstractClassWithDefaultConstructor
        { 
        
        }

        public abstract class AbstractClassWithHiddenConstructor
        {
            private AbstractClassWithHiddenConstructor()
            { }
        }

        public class ClassWithHiddenConstructor
        {
            private ClassWithHiddenConstructor()
            {
            
            }
        }

        public sealed class SealedClass
        {
        
        }

        public class ClassWithDefaultConstructor
        { 
        
        }
    }
}
