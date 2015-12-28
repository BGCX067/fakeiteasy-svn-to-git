namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Api;

    /// <summary>
    /// Responsible for creating fake objects, can be proxied objects or objects
    /// resolved from the IFakeObjectContainer.
    /// </summary>
    internal class FakeObjectFactory
    {
        private IProxyGenerator proxyGenerator;
        private IFakeObjectContainer container;
        private FakeObject.Factory fakeObjectFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObjectFactory"/> class.
        /// </summary>
        /// <param name="container">The container to use.</param>
        /// <param name="proxyGenerator">The proxy generator to use.</param>
        /// <param name="fakeObjectFactory">The fake object factory to use.</param>
        public FakeObjectFactory(IFakeObjectContainer container, IProxyGenerator proxyGenerator, FakeObject.Factory fakeObjectFactory)
        {
            this.container = container;
            this.proxyGenerator = proxyGenerator;
            this.fakeObjectFactory = fakeObjectFactory;
        }

        /// <summary>
        /// Creates a fake object.
        /// </summary>
        /// <param name="typeOfFake">The type of fake.</param>
        /// <param name="argumentsForConstructor">Of the faked type if any. Specify null when no arguments are provided.</param>
        /// <param name="allowNonProxiedFakes">If set to <c>true</c> fakes resolved for the IFakeObjectContainer are allowed.</param>
        /// <returns>The created fake object.</returns>
        public virtual object CreateFake(Type typeOfFake, IEnumerable<object> argumentsForConstructor, bool allowNonProxiedFakes)
        {
            object result = null;
            if (FakesFromContainerAreAllowed(allowNonProxiedFakes, argumentsForConstructor) && this.container.TryCreateFakeObject(typeOfFake, out result))
            {
                return result;
            }

            return this.GenerateProxy(typeOfFake, argumentsForConstructor);
        }

        private object GenerateProxy(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            var fakeObject = this.CreateFakeObject();

            ProxyResult result = null;
            if (argumentsForConstructor != null)
            {
                result = this.GenerateProxyWithArgumentsForConstructor(typeOfFake, argumentsForConstructor, fakeObject, result);
            }
            else
            {
                result = this.GenerateProxyWithoutArgumentsForConstructor(typeOfFake, fakeObject, result);
            }

            fakeObject.SetProxy(result);

            this.container.ConfigureFake(typeOfFake, result.Proxy);

            return result.Proxy;
        }

        private static bool FakesFromContainerAreAllowed(bool allowNonProxiedFakes, IEnumerable<object> argumentsForConstructor)
        {
            return allowNonProxiedFakes && argumentsForConstructor == null;
        }

        private FakeObject CreateFakeObject()
        {
            return this.fakeObjectFactory.Invoke();
        }

        private ProxyResult GenerateProxyWithoutArgumentsForConstructor(Type typeOfFake, FakeObject fake, ProxyResult result)
        {
            if (!this.proxyGenerator.TryGenerateProxy(typeOfFake, fake, this.container, out result))
            {
                ThrowCanNotGenerateFakeException(typeOfFake);
            }

            return result;
        }

        private ProxyResult GenerateProxyWithArgumentsForConstructor(Type typeOfFake, IEnumerable<object> argumentsForConstructor, FakeObject fake, ProxyResult result)
        {
            if (!this.proxyGenerator.TryGenerateProxy(typeOfFake, fake, argumentsForConstructor, out result))
            {
                ThrowCanNotGenerateFakeException(typeOfFake);
            }

            return result;
        }

        private static void ThrowCanNotGenerateFakeException(Type typeOfFake)
        {
            throw new ArgumentException(ExceptionMessages.CanNotGenerateFakeMessage.FormatInvariant(typeOfFake));
        }
    }
}