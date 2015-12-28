using FakeItEasy.Api;
using FakeItEasy.DynamicProxy;
using FakeItEasy.Tests.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using FakeItEasy.Api;
using System.IO;
using System.Reflection;

namespace FakeItEasy.Tests.DynamicProxy
{
    [TestFixture]
    public class DynamicProxyProxyGeneratorTests
    {
        FakeObject fakeObject;
        IFakeObjectContainer container;

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

        private MemberInfo[] nonInterceptableMembers = new MemberInfo[] 
        {
            typeof(string).GetMethod("GetType"),
            typeof(string).GetProperty("Length"),
            typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetGetMethod(),
            typeof(TypeWithNonVirtualProperty).GetProperty("Foo").GetSetMethod(),
            typeof(TypeWithNonVirtualProperty).GetProperty("Foo")
        };

        private MemberInfo[] interceptableMembers = new MemberInfo[] 
        {
            typeof(IFoo).GetMethod("Bar", new Type[] {}),
            typeof(IFoo).GetProperty("SomeProperty").GetGetMethod(),
            typeof(IFoo).GetProperty("SomeProperty").GetSetMethod(),
            typeof(IFoo).GetProperty("SomeProperty"),
            typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            typeof(TypeWithInternalInterceptableProperties).GetProperty("WriteOnly", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            typeof(TypeWithInternalInterceptableProperties).GetProperty("Normal", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
            typeof(TypeWithInternalInterceptableProperties).GetProperty("ReadOnlyAutomatic", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        };

        public class TypeWithInternalInterceptableProperties
        {
            internal virtual string ReadOnly
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            internal virtual string WriteOnly
            {
                set
                {
                    throw new NotImplementedException();
                }
            }

            internal virtual string Normal
            {
                get;
                set;
            }

            internal virtual string ReadOnlyAutomatic
            {
                get;
                private set;
            }
        }

        [TestCaseSource("nonInterceptableMembers")]
        public void MemberCanBeIntercepted_should_return_false_for_non_virtual_member(MemberInfo member)
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.False);
        }

        [TestCaseSource("interceptableMembers")]
        public void MemberCanBeIntercepted_should_return_true_for_virtual_member(MemberInfo member)
        {
            // Arrange
            var generator = this.CreateGenerator();

            // Act
            var result = generator.MemberCanBeIntercepted(member);

            // Assert
            Assert.That(result, Is.True, "Was not able to intercept the member");
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_return_true_when_type_can_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_assign_result_to_out_parameter(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, this.fakeObject, this.container);

            Assert.That(result.Proxy, Is.InstanceOf(typeOfProxy));
        }

        [TestCaseSource("typesThatCanBeProxied")]
        public void GenerateProxy_should_return_proxy_that_can_get_the_passed_in_proxy_manager(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeOfProxy, this.fakeObject, this.container);

            Assert.That(result.Proxy.FakeObject, Is.SameAs(this.fakeObject));

        }

        [TestCaseSource("typesThatCanNotBeProxied")]
        public void GenerateProxy_should_return_false_for_types_that_can_not_be_proxied(Type typeOfProxy)
        {
            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeOfProxy, this.fakeObject, this.container);
            
            Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        }

        [Test]
        public void GenerateProxy_should_throw_when_type_is_interface_but_arguments_for_constructor_is_specified()
        {
            var generator = this.CreateGenerator();

            var thrown = Assert.Throws<ArgumentException>(() =>
                generator.GenerateProxy(typeof(IFoo), this.fakeObject, new object[] { 1, 2 }));

            Assert.That(thrown.Message, Is.EqualTo("Arguments for constructor was specified when generating proxy of interface type."));
        }

        [TestCase(typeof(ISomeInterface))]
        [TestCase(typeof(SomeInterfaceImplementation))]
        [TestCase(typeof(SomeAbstractInterfaceImplementation))]
        public void GenerateProxy_should_generate_result_that_raises_events_when_calls_are_intercepted(Type someInterfaceType)
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(someInterfaceType, this.fakeObject, this.container);

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
        public void GenerateProxy_should_return_true_when_generating_class_with_arguments_for_constructor_that_matches_constructor()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" });

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GenerateProxy_with_arguments_for_constructor_should_generate_proxies_raises_events_when_calls_made_to_proxy()
        {
            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" });

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
        public void GenerateProxy_with_arguments_for_constructor_should_generate_proxies_that_can_get_proxy_manager()
        {
            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, new object[] { "foo" });

            Assert.That(result.Proxy.FakeObject, Is.SameAs(this.fakeObject));
        }

        [Test]
        public void GenerateProxy_without_arguments_for_constructor_should_return_true_when_container_can_resolve_arguments_for_constructor()
        {
            A.CallTo(() => this.container.TryCreateFakeObject(typeof(string), out Null<object>.Out)).Returns(true).AssignsOutAndRefParameters("foo");

            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_container_can_resolve_arguments_for_constructor()
        {
            A.CallTo(() => this.container.TryCreateFakeObject(typeof(string), out Null<object>.Out)).Returns(true).AssignsOutAndRefParameters("foo");

            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatTakesSingleString), this.fakeObject, this.container);

            Assert.That(result.Proxy, Is.InstanceOf<TypeWithConstructorThatTakesSingleString>());
        }

        [Test]
        public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_value_type()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeThatTakesValueTypeInConstructor), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_interface_type()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeThatTakesProxyableTypeInConstructor), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GenerateProxy_without_arguments_for_constructor_should_generate_proxy_when_constructor_argument_is_class_with_default_constructor()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(SealedTypeThatTakesClassWithDefaultConstructorInConstructor), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        [Test]
        public void GeneratedProxies_should_be_serializable()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container);

            Assert.That(result, Is.BinarySerializable);
        }

        [Test]
        public void GeneratedProxies_should_intercept_calls_to_ToString()
        {
            bool wasIntercepted = false;

            var generator = this.CreateGenerator();

            var result =  generator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container);

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

            var result = generator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container);

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

            var result = generator.GenerateProxy(typeof(IFoo), this.fakeObject, this.container);

            result.CallWasIntercepted += (s, e) =>
            {
                wasIntercepted = true;
                e.Call.SetReturnValue(1);
            };
            
            result.Proxy.GetHashCode();

            Assert.That(wasIntercepted, Is.EqualTo(true));
        }

        [Test]
        public void GenerateProxy_should_return_false_when_resolvable_constructors_contains_circular_dependencies()
        {
            var generator = this.CreateGenerator();
            var result = generator.GenerateProxy(typeof(CircularClassA), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        }

        [Test]
        public void GenerateProxy_should_use_widest_fakeable_constructor()
        {
            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeof(ClassWithDefaultConstructorAndResolvableConstructor), this.fakeObject, this.container);

            var proxy = result.Proxy as ClassWithDefaultConstructorAndResolvableConstructor;

            Assert.That(proxy.WidestConstructorWasCalled, Is.True);
        }

        [Test]
        public void GenerateProxy_should_return_false_when_resolved_constructor_throws_exception()
        {
            var generator = this.CreateGenerator();
            
            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatThrows), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.False);
        }

        [Test]
        public void GenerateProxy_should_return_true_when_type_has_constructor_that_does_not_throw()
        {
            var generator = this.CreateGenerator();

            var result = generator.GenerateProxy(typeof(TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow), this.fakeObject, this.container);

            Assert.That(result.ProxyWasSuccessfullyCreated, Is.True);
        }

        public class TypeWithConstructorThatThrows
        {
            public TypeWithConstructorThatThrows(IServiceProvider a)
            {
                throw new Exception();
            }
        }

        public class TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow
        {
            public TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow(IServiceProvider a)
            {
                throw new Exception();
            }

            public TypeWithConstructorThatThrowsAndConstructorThatDoesNotThrow()
            {

            }
        }

        public class ClassWithDefaultConstructorAndResolvableConstructor
        {
            public bool WidestConstructorWasCalled = false;

            public ClassWithDefaultConstructorAndResolvableConstructor()
            { }

            public ClassWithDefaultConstructorAndResolvableConstructor(ISomeInterface a)
            { }

            public ClassWithDefaultConstructorAndResolvableConstructor(ISomeInterface a, IFoo b)
            {
                this.WidestConstructorWasCalled = true;
            }
        }

        public class CircularClassA
        { 
            public CircularClassA(CircularClassB b)
            {}
        }

        public class CircularClassB
        { 
            public CircularClassB(CircularClassA a)
            {
            
            }
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

        public class SealedTypeThatTakesClassWithDefaultConstructorInConstructor
        {
            public SealedTypeThatTakesClassWithDefaultConstructorInConstructor(TypeWithDefaultConstructor a)
            {
                
            }
        }

        public sealed class TypeWithDefaultConstructor
        {
            public TypeWithDefaultConstructor()
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

        public class TypeWithNonVirtualProperty
        {
            public int Foo { get; set; }
        }
    }
}
