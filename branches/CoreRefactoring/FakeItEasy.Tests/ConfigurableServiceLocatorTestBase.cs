using FakeItEasy.Extensibility;
using NUnit.Framework;
using System;
using FakeItEasy.Api;

namespace FakeItEasy.Tests
{
    public abstract class ConfigurableServiceLocatorTestBase
    {
        protected DelegateFakeObjectContainer container;
        private ServiceLocator replacedServiceLocator;
        protected readonly IExtensibleIs Its;
        private IDisposable scope;

        [SetUp]
        public void SetUp()
        {
            this.container = new DelegateFakeObjectContainer();
            this.container.Register<FakeObject.Factory>(() => ServiceLocator.Current.Resolve<FakeObject.Factory>());

            this.scope = Fake.CreateScope(this.container);

            this.replacedServiceLocator = ServiceLocator.Current;
            ServiceLocator.Current = A.Fake<ServiceLocator>(ServiceLocator.Current);

            this.OnSetUp();
        }

        protected abstract void OnSetUp();

        [TearDown]
        public void TearDown()
        {
            ServiceLocator.Current = this.replacedServiceLocator;
            this.scope.Dispose();
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
