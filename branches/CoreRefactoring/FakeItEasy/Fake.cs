namespace FakeItEasy
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Api;
    using FakeItEasy.Assertion;
    using FakeItEasy.Configuration;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides static methods for accessing fake objects.
    /// </summary>
    public static class Fake
    {
        /// <summary>
        /// Gets an object that provides a fluent interface syntax for configuring
        /// the fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakedObject">The fake object to configure.</param>
        /// <returns>A configuration object.</returns>
        /// <exception cref="ArgumentNullException">The fakedObject was null.</exception>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        public static IFakeConfiguration<TFake> Configure<TFake>(TFake fakedObject)
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            var factory = ServiceLocator.Current.Resolve<IFakeConfigurationFactory>();
            return factory.Create<TFake>(GetFakeObject(fakedObject));
        }

        [DebuggerStepThrough]
        internal static FakeObject GetFakeObject(object fakedObject)
        {
            var accessor = fakedObject as IFakedProxy;

            if (accessor == null)
            {
                throw new ArgumentException(ExceptionMessages.ConfiguringNonFakeObjectExceptionMessage, "fakedObject");
            }

            return accessor.GetFakeObject();
        }

        /// <summary>
        /// Gets an object that provides assertions for the specified fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakedObject">The fake object to get assertions for.</param>
        /// <returns>An assertion object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        [DebuggerStepThrough]
        public static IFakeAssertions<TFake> Assert<TFake>(TFake fakedObject)
        {
            var factory = ServiceLocator.Current.Resolve<IFakeAssertionsFactory>();
            return factory.CreateAsserter<TFake>(GetFakeObject(fakedObject));
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static IDisposable CreateScope()
        {
            return FakeScope.Create();
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When inside a scope the
        /// getting the calls made to a fake will return only the calls within that scope and when
        /// asserting that calls were made, the calls must have been made within that scope.
        /// </summary>
        /// <param name="container">The container to use within the specified scope.</param>
        /// <returns>The created scope.</returns>
        public static IDisposable CreateScope(IFakeObjectContainer container)
        {
            return FakeScope.Create(container);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are equal.
        /// </summary>
        /// <param name="objA">The first object to compare.</param>
        /// <param name="objB">The second object to compare.</param>
        /// <returns>True if the two objects are equal.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
        public new static bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Gets a value indicating if the two objects are the same reference.
        /// </summary>
        /// <param name="objA">The obj A.</param>
        /// <param name="objB">The obj B.</param>
        /// <returns>True if the objects are the same reference.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
        public new static bool ReferenceEquals(object objA, object objB)
        {
            return object.ReferenceEquals(objA, objB);
        }

        /// <summary>
        /// Gets all the calls made to the specified fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the faked object.</typeparam>
        /// <param name="fakedObject">The faked object.</param>
        /// <returns>A collection containing the calls to the object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        public static ICallCollection<TFake> GetCalls<TFake>(TFake fakedObject)
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            var factory = ServiceLocator.Current.Resolve<ICallCollectionFactory>();
            return factory.CreateCallCollection<TFake>(Fake.GetFakeObject(fakedObject));
        }

        internal static FakeObjectFactory CreateFactory()
        {
            return ServiceLocator.Current.Resolve<FakeObjectFactory>();
        }
    }
}