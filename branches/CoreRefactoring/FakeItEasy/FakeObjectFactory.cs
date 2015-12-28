namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Api;

    internal class FakeObjectFactory
    {
        private IProxyGenerator proxyGenerator;
        private IFakeObjectContainer container;
        private FakeObject.Factory fakeObjectFactory;

        public FakeObjectFactory(IFakeObjectContainer container, IProxyGenerator proxyGenerator, FakeObject.Factory fakeObjectFactory)
        {
            this.container = container;
            this.proxyGenerator = proxyGenerator;
            this.fakeObjectFactory = fakeObjectFactory;
        }

        public virtual object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor, bool allowNonProxiedFakes)
        {
            object result = null;
            if (allowNonProxiedFakes && this.container.TryCreateFakeObject(typeOfFake, argumentsForConstructor, out result))
            {
                return result;
            }

            return this.GenerateProxy(typeOfFake, argumentsForConstructor);
        }

        private object GenerateProxy(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            var fake = this.fakeObjectFactory.Invoke();

            ProxyResult result = null;
            if (argumentsForConstructor != null)
            {
                if (!this.proxyGenerator.TryGenerateProxy(typeOfFake, fake, argumentsForConstructor, out result))
                {
                    throw new ArgumentException(ExceptionMessages.CanNotGenerateFakeMessage);
                }
            }
            else
            {
                if (!this.proxyGenerator.TryGenerateProxy(typeOfFake, fake, this.container, out result))
                {
                    throw new ArgumentException(ExceptionMessages.CanNotGenerateFakeMessage);
                }
            }

            fake.SetProxy(result);
            return result.Proxy;
        }
    }
}