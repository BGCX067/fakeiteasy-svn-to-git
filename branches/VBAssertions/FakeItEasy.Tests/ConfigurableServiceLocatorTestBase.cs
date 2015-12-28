using FakeItEasy.Extensibility;
using NUnit.Framework;
using System;

namespace FakeItEasy.Tests
{
    public abstract class ConfigurableServiceLocatorTestBase
    {
        private ServiceLocator replacedServiceLocator;
        protected readonly IExtensibleIs Its;

        [SetUp]
        public void SetUp()
        {
            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(ServiceLocator.Current);

            this.OnSetUp();
        }

        protected abstract void OnSetUp();

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
        }

        protected void StubResolve<T>(object returnedInstance)
        {
            Fake.Configure(ServiceLocator.Current)
                .CallsTo(x => x.Resolve(typeof(T)))
                .Returns(returnedInstance);
        }

        protected T StubResolveWithFake<T>()
        {
            var result = A.Fake<T>();

            this.StubResolve<T>(result);

            return result;
        }
    }
}
