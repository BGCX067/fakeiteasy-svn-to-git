namespace FakeItEasy.ExtensionSyntax.Full
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq.Expressions;
    using FakeItEasy.Assertion;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extension methods for configuring and asserting on faked objects
    /// without going through the static methods of the Fake-class.
    /// </summary>
    public static class FullExtensionSyntax
    {
        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <typeparam name="TMember">The type of the return value of the member.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        public static IReturnValueArgumentValidationConfiguration<TMember> CallsTo<TFake, TMember>(this TFake fakedObject, Expression<Func<TFake, TMember>> callSpecification) where TFake : class
        {
            Guard.IsNotNull(callSpecification, "callSpecification");
            
            return Configure.Fake(fakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidArgumentValidationConfiguration CallsTo<TFake>(this TFake fakedObject, Expression<Action<TFake>> callSpecification) where TFake : class
        {
            Guard.IsNotNull(callSpecification, "callSpecification");

            return Configure.Fake(fakedObject).CallsTo(callSpecification);
        }

        /// <summary>
        /// Configures the behavior of the fake object when a call is made to any method on the
        /// object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns>A configuration object.</returns>
        public static IVoidConfiguration AnyCall<TFake>(this TFake fakedObject)
        {
            return Configure.Fake(fakedObject).AnyCall();
        }

        /// <summary>
        /// Gets an object that provides assertions for the specified fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakedObject">The fake object to get assertions for.</param>
        /// <returns>An assertion object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        public static IFakeAssertions<TFake> Assert<TFake>(this TFake fakedObject) where TFake : class
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            return Fake.Assert(fakedObject);
        }
    }
}
