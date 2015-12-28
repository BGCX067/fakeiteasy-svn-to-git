namespace FakeItEasy.Api
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;

    /// <summary>
    /// Defines the interface for objects that can generate fake proxy objects.
    /// </summary>
    internal interface IFakeProxyGenerator
    {
        /// <summary>
        /// Generates a fake proxy.
        /// </summary>
        /// <param name="fakeObject">The fake object the proxy belongs to.</param>
        /// <param name="interceptionCallback">A callback method the proxy should call when a call is intercepted.</param>
        /// <param name="type">The type of proxy to generate.</param>
        /// <param name="argumentsForConstructor">Arguments for the constructor of the proxied type.</param>
        /// <returns>The created proxy.</returns>
        IFakedProxy GenerateFake(FakeObject fakeObject, Action<IWritableFakeObjectCall> interceptionCallback, Type type, params object[] argumentsForConstructor);

        /// <summary>
        /// Determines whether this instance can create a proxy for the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        /// <c>true</c> if this instance can create a proxy for the specified type; otherwise, <c>false</c>.
        /// </returns>
        bool CanCreateProxyOfType(Type type);
    }
}
