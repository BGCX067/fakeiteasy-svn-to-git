using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Configuration;
using System.Linq.Expressions;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class FakeTests
        : ConfigurableServiceLocatorTestBase
    {
        [Test]
        public void GetCalls_gets_all_calls_to_fake_object()
        {
            var foo = A.Fake<IFoo>();

            foo.Bar();
            foo.Baz();
            foo.Biz();

            var calls = Fake.GetCalls(foo);
            var namesOfCalledMethods = calls.Select(x => x.Method.Name).ToArray();

            Assert.That(namesOfCalledMethods, Is.EquivalentTo(new[] { "Bar", "Baz", "Biz" }));            
        }

        [Test]
        public void GetCalls_is_properly_guarded()
        {
            NullGuardedConstraint.Assert(() => Fake.GetCalls(A.Fake<IFoo>()));
        }

        [Test]
        public void Static_equals_delegates_to_static_method_on_object()
        {
            Assert.That(Fake.Equals("foo", "foo"), Is.True);
        }

        [Test]
        public void Static_ReferenceEquals_delegates_to_static_method_on_object()
        {
            var s = "";

            Assert.That(Fake.ReferenceEquals(s, s), Is.True);
        }

        [Test]
        public void CreateScope_should_create_a_new_scope()
        {
            var originalScope = FakeScope.Current;
            using (Fake.CreateScope(A.Fake<IFakeObjectContainer>()))
            {
                Assert.That(FakeScope.Current, Is.Not.SameAs(originalScope));
            }
        }

        [Test]
        public void GetFakeObject_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                Fake.GetFakeObject(A.Fake<IFoo>()));
        }

        [Test]
        public void CastTo_should_resolve_fake_cast_manager()
        {
            // Arrange
            this.StubResolveWithFake<IFakeCastManager>();

            // Act
            var foo = A.Fake<IFoo>();
            Fake.CastTo(typeof(IBar), foo);

            // Assert
            A.CallTo(() => ServiceLocator.Current.Resolve(typeof(IFakeCastManager))).Assert(Happened.Once);
        }

        [Test]
        public void CastTo_should_call_cast_to_on_fake_manager()
        {
            // Arrange
            var foo = A.Fake<IFoo>();
            var bar = A.Fake<IBar>();

            var manager = this.StubResolveWithFake<IFakeCastManager>();
            
            // Act
            Fake.CastTo(typeof(IBar), foo);

            // Assert
            A.CallTo(() => manager.CastTo(typeof(IBar), foo)).Assert(Happened.Once);
        }

        [Test]
        public void CastTo_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                Fake.CastTo(typeof(IBar), A.Fake<IFoo>()));
        }

        [Test]
        public void Generic_CastTo_should_resolve_fake_cast_manager()
        {
            // Arrange
            var bar = A.Fake<IBar>();

            var manager = this.StubResolveWithFake<IFakeCastManager>();

            // Act
            var foo = new FakeThatImplementsIBar(A.Fake<IFoo>());
            Fake.CastTo<IBar>(foo);

            // Assert
            A.CallTo(() => ServiceLocator.Current.Resolve(typeof(IFakeCastManager))).Assert(Happened.Once);
        }

        [Test]
        public void Generic_CastTo_should_call_CastTo_on_fake_manager()
        {
            // Arrange
            var foo = new FakeThatImplementsIBar(A.Fake<IFoo>());

            var manager = this.StubResolveWithFake<IFakeCastManager>();
            

            // Act
            Fake.CastTo<IBar>(foo);

            // Assert
            A.CallTo(() => manager.CastTo(typeof(IBar), foo)).Assert(Happened.Once);
        }

        [Test]
        public void GenericCastTo_should_return_passed_in_object()
        {
            // Arrange
            var foo = new FakeThatImplementsIBar(A.Fake<IFoo>());

            var manager = this.StubResolveWithFake<IFakeCastManager>();


            // Act
            var result = Fake.CastTo<IBar>(foo);

            // Assert
            Assert.That(result, Is.SameAs(foo));
        }

        [Test]
        public void GenericCastTo_should_be_null_guarded()
        {
            // Arrange

            // Act

            // Assert
            NullGuardedConstraint.Assert(() =>
                Fake.CastTo<IBar>(A.Fake<IFoo>()));
        }

        private class FakeThatImplementsIBar
            : IBar, IFakedProxy
        {
            public FakeThatImplementsIBar(object fake)
            {
                this.FakeObject = Fake.GetFakeObject(fake);
            }

            public void Foo()
            {
                throw new NotImplementedException();
            }

            public FakeObject FakeObject
            {
                get;
                private set;
            }
        }

    }
}
