using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Linq.Expressions;
using Castle.Core.Interceptor;
using FakeItEasy.Configuration;
using FakeItEasy.Api;
using System.Collections;
using FakeItEasy.Expressions;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class CallCollectionTests
    {
        private CallCollection<TFake> CreateCollection<TFake>(FakeObject fake)
        {
            var factory = ServiceLocator.Current.Resolve<ICallCollectionFactory>();
            return factory.CreateCallCollection<TFake>(fake);
        }

        [Test]
        public void Constructor_should_be_null_guarded()
        {
            NullGuardedConstraint.Assert(() =>
                new CallCollection<IFoo>(new FakeObject(typeof(IFoo)), x => (ExpressionCallMatcher)null));
        }

        [Test]
        public void FakedObject_should_return_fake_object()
        {
            var foo = new FakeObject(typeof(IFoo));

            var collection = this.CreateCollection<IFoo>(foo);

            Assert.That(collection.FakedObject, Is.SameAs(foo.Object));
        }

        [Test]
        public void Constructor_should_set_fake_to_Fake_property()
        {
            var foo = new FakeObject(typeof(IFoo));

            var collection = this.CreateCollection<IFoo>(foo);

            Assert.That(collection.Fake, Is.EqualTo(foo));
        }

        [Test]
        public void GetEnumerator_should_return_an_enumerator_that_enumerates_all_calls_in_the_fake_object()
        {
            var foo = new FakeObject(typeof(IFoo));

            ((IFoo)foo.Object).Bar();
            ((IFoo)foo.Object).Baz();

            var collection = this.CreateCollection<IFoo>(foo);

            Assert.That(collection.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetEnumerator_through_interface_should_return_an_enumerator_that_enumerates_all_calls_in_the_fake_object()
        {
            var foo = new FakeObject(typeof(IFoo));

            ((IFoo)foo.Object).Bar();
            ((IFoo)foo.Object).Baz();

            var collection = (IEnumerable)this.CreateCollection<IFoo>(foo);
            var enumerator = collection.GetEnumerator();
            int count = 0;
            while (enumerator.MoveNext())
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void Matching_should_return_the_calls_matching_the_specified_expression_when_call_has_no_return_value()
        {
            var foo = new FakeObject(typeof(IFoo));

            ((IFoo)foo.Object).Bar("foo");
            ((IFoo)foo.Object).Baz();
            ((IFoo)foo.Object).Bar("bar");
            ((IFoo)foo.Object).Bar("foobar");
            
            var collection = this.CreateCollection<IFoo>(foo);

            var matchingCalls = collection.Matching(x => x.Bar(Argument.Is.Matching<string>(s => s.StartsWith("foo"))));

            Assert.That(matchingCalls.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Matching_should_return_the_calls_matching_the_specified_expression_when_call_has_return_value()
        {
            var foo = new FakeObject(typeof(IFoo));

            ((IFoo)foo.Object).Bar("foo");
            ((IFoo)foo.Object).Bar("bar");
            ((IFoo)foo.Object).Baz();
            ((IFoo)foo.Object).Bar("foobar");

            var collection = this.CreateCollection<IFoo>(foo);

            var matchingCalls = collection.Matching(x => x.Baz());

            Assert.That(matchingCalls.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Mathcing_with_func_call_should_be_properly_guarded()
        {
            var foo = new FakeObject(typeof(IFoo));
            var collection = this.CreateCollection<IFoo>(foo);

            NullGuardedConstraint.Assert(() => collection.Matching(x => x.Biz()));
        }

        [Test]
        public void Matching_with_action_call_should_be_properly_guarded()
        {
            var foo = new FakeObject(typeof(IFoo));
            var collection = this.CreateCollection<IFoo>(foo);

            NullGuardedConstraint.Assert(() => collection.Matching(x => x.Bar()));
        }
    }
}
