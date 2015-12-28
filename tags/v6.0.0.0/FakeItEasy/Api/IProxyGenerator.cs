using System;
using System.Collections.Generic;

namespace FakeItEasy.Api
{
    /// <summary>
    /// Represents a generator that can generate proxies that emits events when calls are intercepted.
    /// </summary>
    public interface IProxyGenerator
    {
        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor, out ProxyResult generatedProxy);

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container, out ProxyResult generatedProxy);
    }
}
