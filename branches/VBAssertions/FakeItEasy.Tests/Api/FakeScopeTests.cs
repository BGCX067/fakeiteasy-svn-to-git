using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.Tests.Api
{
    [TestFixture]
    public class FakeScopeTests
    {
        [Test]
        public void Dispose_sets_outside_scope_as_current_scope()
        {
            var scope = FakeScope.Current;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current, Is.Not.EqualTo(scope));
            }

            Assert.That(FakeScope.Current, Is.EqualTo(scope));
        }

        [Test]
        public void Disposing_original_scope_does_nothing()
        {
            FakeScope.Current.Dispose();
        }

        [Test]
        public void OriginalScope_has_NullFakeObjectContainer()
        {
            Assert.That(FakeScope.Current.FakeObjectContainer, Is.InstanceOf<NullFakeObjectContainer>());
        }

        [Test]
        public void CreatingNewScope_without_container_has_container_set_to_same_container_as_parent_scope()
        {
            var parentContainer = FakeScope.Current.FakeObjectContainer;

            using (Fake.CreateScope())
            {
                Assert.That(FakeScope.Current.FakeObjectContainer, Is.SameAs(parentContainer));
            }
        }

        [Test]
        public void CreatingNewScope_with_container_sets_that_container_to_scope()
        {
            var newContainer = A.Fake<IFakeObjectContainer>();

            using (FakeScope.Create(newContainer))
            {
                Assert.That(FakeScope.Current.FakeObjectContainer, Is.SameAs(newContainer));
            }
        }
    }

    [TestFixture]
    public class PredicateCallRuleTests
    {
        private MethodInfoManager methodInfoManager;

        [SetUp]
        public void SetUp()
        {
            this.OnSetUp();
        }

        protected virtual void OnSetUp()
        {
            this.methodInfoManager = A.Fake<MethodInfoManager>();
        }

        private RecordedCallRule CreateRule()
        {
            return new RecordedCallRule(this.methodInfoManager);
        }
    }
}
