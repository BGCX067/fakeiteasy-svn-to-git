namespace FakeItEasy.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides options for generating fake object.
    /// </summary>
    /// <typeparam name="T">The type of fake object generated.</typeparam>
    public interface IFakeBuilderOptionsBuilder<T>
        : IHideObjectMembers
    {
        //IFakeBuilderOptionsBuilder<T> Implementing<TInterface>();

        /// <summary>
        /// Specifies arguments for the constructor of the faked class.
        /// </summary>
        /// <param name="argumentsForConstructor">The arguments to pass to the consturctor of the faked class.</param>
        /// <returns>Options object.</returns>
        IFakeBuilderOptionsBuilder<T> WithArgumentsForConstructor(IEnumerable<object> argumentsForConstructor);

        /// <summary>
        /// Specifies arguments for the constructor of the faked class by giving an expression with the call to
        /// the desired constructor using the arguments to be passed to the constructor.
        /// </summary>
        /// <param name="constructorCall">The constructor call to use when creating a class proxy.</param>
        /// <returns>Options object.</returns>
        IFakeBuilderOptionsBuilder<T> WithArgumentsForConstructor(Expression<Func<T>> constructorCall);

        /// <summary>
        /// Specifies that the fake should delegate calls to the specified instance.
        /// </summary>
        /// <param name="wrappedInstance">The object to delegate calls to.</param>
        /// <returns>Options object.</returns>
        IFakeBuilderOptionsBuilderForWrappers<T> Wrapping(T wrappedInstance);
    }
}
