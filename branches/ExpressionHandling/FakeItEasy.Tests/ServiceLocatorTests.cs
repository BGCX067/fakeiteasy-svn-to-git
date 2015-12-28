using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    [TestFixture]
    public class ServiceLocatorTests
    {
        [Test]
        public void Current_should_not_be_null()
        {
            Assert.That(ServiceLocator.Current, Is.Not.Null);
        }

        [Test]
        public void Resolve_FakeScope_should_return_the_current_scope()
        {
            Assert.That(ServiceLocator.Current.Resolve<FakeScope>(), Is.SameAs(FakeScope.Current));
        }

        private IEnumerable<Type> SingletonTypes
        {
            get
            {
                yield return typeof(FakeItEasy.Expressions.ExpressionCallMatcher.Factory);
                yield return typeof(FakeItEasy.Expressions.ArgumentValidatorFactory);
                yield return typeof(IFakeProxyGenerator);
            }
        }

        private IEnumerable<Type> NonSingletonTypes
        {
            get
            {
                return Enumerable.Empty<Type>();
            }
        }

        [Test]
        public void Should_be_registered_as_singleton([ValueSource("SingletonTypes")] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void Should_be_registered_as_non_singleton([ValueSource("NonSingletonTypes")] Type type)
        {
            var first = ServiceLocator.Current.Resolve(type);
            var second = ServiceLocator.Current.Resolve(type);

            Assert.That(second, Is.Not.SameAs(first));
        }
    }
}
