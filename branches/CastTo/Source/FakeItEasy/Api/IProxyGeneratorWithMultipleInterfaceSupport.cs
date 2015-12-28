namespace FakeItEasy.Api
{
    using System;

    /// <summary>
    /// Represents a proxy generator that can add interfaces to a generated proxy.
    /// </summary>
    public interface IProxyGeneratorWithMultipleInterfaceSupport
        : IProxyGenerator
    {
        /// <summary>
        /// Adds an interface implementation for the specified interface type.
        /// </summary>
        /// <param name="interfaceType">The type of interface to add.</param>
        /// <param name="proxy">The proxy to add the interface to.</param>
        void AddInterfaceToProxy(Type interfaceType, object proxy);
    }
}
