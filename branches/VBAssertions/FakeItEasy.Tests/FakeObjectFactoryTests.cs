using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeObjectFactoryTests
    {
        private DictionaryContainer container;

        private FakeObjectFactory CreateFactory()
        {
            this.container = new DictionaryContainer
            {
                RegisteredTypes = new Dictionary<Type, object>()
            };

            return new FakeObjectFactory(this.container);
        }

        private class DictionaryContainer
            : IFakeObjectContainer
        {
            public IDictionary<Type, object> RegisteredTypes;

            public bool TryCreateFakeObject(Type typeOfFakeObject, IEnumerable<object> arguments, out object fakeObject)
            {
                return this.RegisteredTypes.TryGetValue(typeOfFakeObject, out fakeObject);
            }
        }

        [Test]
        public void CreateFake_should_be_null_guarded()
        {
            var factory = this.CreateFactory();

            NullGuardedConstraint.Assert(() =>
                factory.CreateFake(typeof(IFoo), null, true));    
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new FakeObjectFactory(this.container));
        }

        [Test]
        public void Create_returns_fake_for_interface()
        {
            var factory = this.CreateFactory();
            
            var fake = factory.CreateFake(typeof(IFoo), null, false);

            Assert.That(fake, Is.InstanceOf<IFakedProxy>());
        }

        [Test]
        public void CreateFake_returns_fake_for_abstract_class()
        {
            var factory = this.CreateFactory();

            var fake = factory.CreateFake(typeof(AbstractWidgetFactory), null, false);

            Assert.That(fake, Is.InstanceOf<IFakedProxy>());
        }

        [Test]
        public void CreateFake_returns_fake_for_non_sealed_class()
        {
            var factory = this.CreateFactory();

            var fake = factory.CreateFake(typeof(Bar), null, false);

            Assert.That(fake, Is.Not.Null);
        }

        [Test]
        public void CreateFake_throws_when_type_is_sealed_and_it_can_not_be_created_by_the_fake_container()
        {
            var factory = this.CreateFactory();

            Assert.Throws<InvalidOperationException>(() =>
                factory.CreateFake(typeof(SealedClass), null, false));
        }

        [Test]
        public void CreateFake_with_constructor_specified_returns_fake()
        {
            var factory = this.CreateFactory();

            var fake = factory.CreateFake(typeof(Foo), new[] { "foo" }, false);

            Assert.That(fake, Is.InstanceOf<IFakedProxy>());
        }

        [Test]
        public void CreateFake_with_no_fakeable_constructor_should_throw()
        {
            var factory = this.CreateFactory();
            Assert.Throws<InvalidOperationException>(() =>
                factory.CreateFake(typeof(TypeWithNonFakeableConstructor), null, false));
        }

        #region Move to FakeObjectTests
        // TODO: Move to FakeObjectTests.

        [Test]
        public void CreateFake_returns_serializable_object()
        {
            var factory = this.CreateFactory();
            Assert.That(factory.CreateFake(typeof(IFoo), null, false), Is.BinarySerializable);
        }

        [Test]
        public void CreateFake_object_is_not_serializable_once_it_has_been_configured()
        {
            var factory = this.CreateFactory();

            var foo = factory.CreateFake(typeof(IFoo), null, false) as IFoo;

            Fake.Configure(foo).CallsTo(x => x.Bar()).Returns("test");

            Assert.That(foo, Is.Not.BinarySerializable);
        }

        [Test]
        public void CreateFake_should_return_null_when_a_reference_type_member_that_has_not_been_configured_is_called()
        {
            var foo = A.Fake<IFoo>();

            Assert.That(foo.Bar(), Is.Null);
        }

        [Test]
        public void CreateFake_should_return_default_value_when_a_value_type_member_that_has_not_been_configured_is_called()
        {
            var foo = A.Fake<IFoo>();

            Assert.That(foo.Count, Is.EqualTo(0));
        }
        #endregion

        [Test]
        public void CreateFake_with_arguments_for_constructor_should_throw_if_no_constructor_was_found_matching_the_arguments()
        {
            var factory = this.CreateFactory();

            Assert.Throws<MissingMethodException>(() =>
                factory.CreateFake(typeof(AbstractClassWithNoDefaultConstructor), new object[] { 0, "this should've been an int" }, false));
        }

        [Test]
        public void CreateFake_with_arguments_for_constructor_should_return_faked_object()
        {
            var factory = this.CreateFactory();

            var faked = factory.CreateFake(typeof(AbstractClassWithNoDefaultConstructor), new object[] { "foo", 1 }, true);

            Assert.That(faked, Is.InstanceOf<IFakedProxy>());
        }

        [Test]
        public void CreateFake_should_return_fake_from_container_when_container_can_create_fake_and_non_proxied_objects_are_allowed()
        {
            var factory = this.CreateFactory();
            this.container.RegisteredTypes.Add(typeof(int), 10);

            var faked = (int)factory.CreateFake(typeof(int), null, true);

            Assert.That(faked, Is.EqualTo(10));
        }

        [Test]
        public void CreateFake_should_not_return_fake_from_container_when_container_can_create_fake_but_non_proxied_objects_are_not_allowed()
        {
            var factory = this.CreateFactory();
            this.container.RegisteredTypes.Add(typeof(IFoo), A.Fake<IFoo>());

            var faked = (IFoo)factory.CreateFake(typeof(IFoo), null, false);

            Assert.That(faked, Is.Not.SameAs(this.container.RegisteredTypes[typeof(IFoo)]));
        }

      

        public class Foo
        {
            public Foo(string bar)
            {

            }

            public virtual string Something()
            {
                throw new NotImplementedException();
            }
        }

        public sealed class SealedClass
        {

        }

        public class TypeWithNonFakeableConstructor
        {
            public TypeWithNonFakeableConstructor(int argument)
            {
                
            }
        }

        public class Bar
        {

        }

        public class WidgetFactory
            : IWidgetFactory
        {

            public virtual IWidget CreateWidget()
            {
                throw new NotImplementedException();
            }
        }

        public interface IWidgetFactory
        {
            IWidget CreateWidget();
        }

        public interface IWidget
        { }

        public abstract class AbstractWidgetFactory
        {

        }

        public abstract class AbstractClassWithNoDefaultConstructor
        {
            protected AbstractClassWithNoDefaultConstructor(string foo, int bar)
            {

            }
        }

        public interface IFoo
        {
            string Bar();
            string Bar(string value);
            string Bar(string s, int i);
            void Baz();

            int Count { get; set; }
        }

        public interface IMyRemoteService
        {
            string ExecuteRequest(string request);
        }
    }
}
