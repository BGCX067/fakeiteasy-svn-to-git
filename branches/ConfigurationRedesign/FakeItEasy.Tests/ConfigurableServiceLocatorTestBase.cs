using FakeItEasy.Extensibility;
using NUnit.Framework;
using System;
using FakeItEasy.Api;
using FakeItEasy.Mef;

namespace FakeItEasy.Tests
{
    public abstract class ConfigurableServiceLocatorTestBase
    {
        private ServiceLocator replacedServiceLocator;
        private IDisposable scope;

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

        protected void StubResolve<T>(T returnedInstance)
        {
            Configure.Fake(ServiceLocator.Current)
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

    public class FakeObjectFactoryDefinition : FakeDefinition<FakeObject.Factory>
    {
        protected override FakeObject.Factory CreateFake()
        {
            return ServiceLocator.Current.Resolve<FakeObject.Factory>();
        }
    }

}
