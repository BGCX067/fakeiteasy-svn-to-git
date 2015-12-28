using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using FakeItEasy.Api;
using System.Diagnostics;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ATests
    {
        [Test]
        public void Fake_returns_fake_for_interface()
        {
            var fake = A.Fake<IWidgetFactory>();

            Assert.That(fake, Is.Not.Null);
        }

        [Test]
        public void Fake_returns_fake_for_abstract_class()
        {
            var fake = A.Fake<AbstractWidgetFactory>();
            
            Assert.That(fake, Is.Not.Null);
        }

        [Test]
        public void Fake_returns_fake_for_non_sealed_class()
        {
            var fake = A.Fake<Bar>();

            Assert.That(fake, Is.Not.Null);
        }

        [Test]
        public void Fake_throws_when_type_is_sealed()
        {
            Assert.Throws<InvalidOperationException>(() =>
                A.Fake<SealedClass>());
        }

        [Test]
        public void Fake_returns_serializable_object()
        {
            Assert.That(A.Fake<IFoo>(), Is.BinarySerializable);
        }

        [Test]
        public void Fake_object_is_not_serializable_once_it_has_been_configured()
        {
            var foo = A.Fake<IFoo>();

            Fake.Configure(foo).CallsTo(x => x.Bar()).Returns("test");

            Assert.That(foo, Is.Not.BinarySerializable);
        }

        [Test]
        public void Fake_with_constructor_specified_returns_fake()
        {
            var foo = A.Fake<Foo>(() => new Foo("foo"));

            var fake = this.GetFakeObject(foo);

            Assert.That(fake, Is.Not.Null);
        }

        [Test]
        public void Fake_with_no_default_constructor_should_throw()
        {
            Assert.Throws<InvalidOperationException>(() =>
                A.Fake<Foo>());
        }

        [Test]
        public void Fake_with_constructor_call_throws_when_passed_in_expression_is_not_constructor_call()
        {
            Assert.Throws<ArgumentException>(() =>
                A.Fake<Foo>(() => CreateFoo()));
        }

        [Test]
        public void Fake_should_return_null_when_a_reference_type_member_that_has_not_been_configured_is_called()
        {
            var foo = A.Fake<IFoo>();

            Assert.That(foo.Bar(), Is.Null);
        }

        [Test]
        public void Fake_should_return_default_value_when_a_value_type_member_that_has_not_been_configured_is_called()
        {
            var foo = A.Fake<IFoo>();

            Assert.That(foo.Count, Is.EqualTo(0));
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
        public void Fake_called_with_instance_returns_wrapping_fake_that_delegates_to_wrapped_object()
        {
            var wrapped = A.Fake<IFoo>();

            var wrapper = A.Fake(wrapped);

            Fake.Configure(wrapped).CallsTo(x => x.Bar()).Returns("foo");

            Assert.That(wrapper.Bar(), Is.EqualTo("foo"));
        }

        [Test]
        public void Fake_with_wrapped_instance_will_override_behavior_of_wrapped_object_on_configured_methods()
        {
            var wrapped = A.Fake<IFoo>();
            var wrapper = A.Fake(wrapped);

            Fake.Configure(wrapped).CallsTo(x => x.Bar()).Returns("wrapped");
            Fake.Configure(wrapper).CallsTo(x => x.Bar()).Returns("wrapper");

            Assert.That(wrapper.Bar(), Is.EqualTo("wrapper"));
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
        public void Fake_with_arguments_for_constructor_should_throw_if_no_constructor_was_found_matching_the_arguments()
        {
            Assert.Throws<MissingMethodException>(() =>
                A.Fake<AbstractClassWithNoDefaultConstructor>(new object[] { 0, "this should've been an int" }));
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_return_faked_object()
        {
            var faked = A.Fake<AbstractClassWithNoDefaultConstructor>(new object[] { "foo", 1 });

            Assert.That(Fake.GetFakeObject(faked), Is.Not.Null);
        }

        [Test]
        public void Fake_with_arguments_for_constructor_should_be_properly_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                A.Fake<AbstractClassWithNoDefaultConstructor>(new object[] { "foo", 1 }));
        }

        //[Test]
        //public void test_name()
        //{
        //    var realService = A.Fake<IMyRemoteService>();
        //    Fake.Configure(realService).CallsTo(x => x.ExecuteRequest("foo")).Returns("bar");

        //    var fakeService = A.Fake(realService, fakePersistence);

        //    fakeService.ExecuteRequest("foo");

        //    Fake.Configure(realService).CallsTo(x => x.ExecuteRequest("foo")).Throws(new NotSupportedException());

        //    Assert.That(fakeService.ExecuteRequest("foo"), Is.EqualTo("bar"));
           
        //}

        private FakeObject GetFakeObject(object faked)
        {
            var accessor = faked as IFakedObject;
            
            return accessor != null ? accessor.GetFakeObject() : null;
        }

        private static Foo CreateFoo()
        {
            return new Foo("foo");
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
