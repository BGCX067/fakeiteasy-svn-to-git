namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using FakeItEasy.Api;
    using System.Linq;

    /// <summary>
    /// Responsible for creating fake objects, can be proxied objects or objects
    /// resolved from the IFakeObjectContainer.
    /// </summary>
    internal class FakeObjectFactory : IFakeObjectGeneratorFactory, IFakeCastManager
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

        /// <summary>
        /// Creates a fake object generator command.
        /// </summary>
        /// <param name="typeOfFake">The type of fake to generate.</param>
        /// <param name="argumentsForConstructor">Of the faked type if any. Specify null when no arguments are provided.</param>
        /// <param name="allowNonProxiedFakes">If set to <c>true</c> fakes resolved from the IFakeObjectContainer are allowed.</param>
        /// <returns>The created command.</returns>
        public IFakeObjectGenerator CreateGenerationCommand(Type typeOfFake, IEnumerable<object> argumentsForConstructor, bool allowNonProxiedFakes)
        {
            return new GeneratorCommand()
            {
                Factory = this,
                TypeOfFake = typeOfFake,
                ArgumentsForConstructor = argumentsForConstructor,
                AllowNonProxiedFakes = allowNonProxiedFakes
            };
        }

        /// <summary>
        /// Casts to.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns></returns>
        public void CastTo(Type interfaceType, object fakedObject)
        {
            AssertThatTypeIsAnInterface(interfaceType);

            if (ObjectImplementsInterface(fakedObject, interfaceType))
            {
                return;
            }

            this.AddInterfaceToProxy(interfaceType, fakedObject);
        }

        private static void AssertThatTypeIsAnInterface(Type interfaceType)
        {
            if (!interfaceType.IsInterface)
            {
                throw new ArgumentException(ExceptionMessages.TypeIsNotInterfaceType, "interfaceType");
            }
        }

        private void AddInterfaceToProxy(Type interfaceType, object fakedObject)
        {
            var generator = this.GetProxyGeneratorWithMultipleInterfacesSupport();
            generator.AddInterfaceToProxy(interfaceType, fakedObject);
        }

        private static bool ObjectImplementsInterface(object value, Type interfaceType)
        {
            return value.GetType().GetInterfaces().Any(x => x.Equals(interfaceType));
        }

        private object GenerateProxy(Type typeOfFake, IEnumerable<object> argumentsForConstructor)
        {
            var fakeObject = this.CreateFakeObject();

            ProxyResult result = null;
            if (argumentsForConstructor != null)
            {
                result = this.GenerateProxyWithArgumentsForConstructor(typeOfFake, argumentsForConstructor, fakeObject);
            }
            else
            {
                result = this.GenerateProxyWithoutArgumentsForConstructor(typeOfFake, fakeObject);
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

        private ProxyResult GenerateProxyWithoutArgumentsForConstructor(Type typeOfFake, FakeObject fake)
        {
            var result = this.proxyGenerator.GenerateProxy(typeOfFake, fake, this.container);

            if (!result.ProxyWasSuccessfullyCreated)
            {
                ThrowCanNotGenerateFakeException(typeOfFake);
            }

            return result;
        }

        private ProxyResult GenerateProxyWithArgumentsForConstructor(Type typeOfFake, IEnumerable<object> argumentsForConstructor, FakeObject fake)
        {
            var result = this.proxyGenerator.GenerateProxy(typeOfFake, fake, argumentsForConstructor);

            if (!result.ProxyWasSuccessfullyCreated)
            {
                ThrowCanNotGenerateFakeException(typeOfFake);
            }

            return result;
        }

        private static void ThrowCanNotGenerateFakeException(Type typeOfFake)
        {
            throw new ArgumentException(ExceptionMessages.CanNotGenerateFakeMessage.FormatInvariant(typeOfFake));
        }

        private IProxyGeneratorWithMultipleInterfaceSupport GetProxyGeneratorWithMultipleInterfacesSupport()
        {
            var generator = this.proxyGenerator as IProxyGeneratorWithMultipleInterfaceSupport;

            if (generator == null)
            {
                throw new NotSupportedException(ExceptionMessages.ProxyGeneratorDoesNotSupportMultipleInterfaces);
            }

            return generator;
        }

        private class GeneratorCommand : IFakeObjectGenerator
        {
            public FakeObjectFactory Factory;
            public Type TypeOfFake;
            public IEnumerable<object> ArgumentsForConstructor;
            public bool AllowNonProxiedFakes;

            public bool GenerateFakeObject()
            {
                try
                {
                    this.GeneratedFake = this.Factory.CreateFake(this.TypeOfFake, this.ArgumentsForConstructor, this.AllowNonProxiedFakes);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            public object GeneratedFake
            {
                get;
                private set;
            }
        }
    }
}