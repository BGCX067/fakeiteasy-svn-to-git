using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Legend.Fakes.Configuration;
using System.ComponentModel;
using Legend.Fakes.Api;
using Legend.Fakes.Assertion;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes
{
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
        public static IFakeConfiguration<TFake> Configure<TFake>(TFake fakedObject) where TFake : class
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            return new FakeConfiguration<TFake>(Fake.GetFakeObject(fakedObject));
        }

        [DebuggerStepThrough]
        internal static FakeObject GetFakeObject(object fakedObject)
        {
            var accessor = fakedObject as IFakeObjectAccessor;

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
        public static IFakeAssertions<TFake> Assert<TFake>(TFake fakedObject) where TFake : class
        {
            return new FakeAsserter<TFake>(GetFakeObject(fakedObject));
        }

        /// <summary>
        /// Creates a new scope and sets it as the current scope. When the scope is disposed
        /// all expectations set up within the scope will be verified.
        /// </summary>
        /// <returns>The created scope.</returns>
        public static IDisposable CreateScope()
        {
            return FakeScope.Create();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
        public static bool Equals(object objA, object objB)
        {
            return object.Equals(objA, objB);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "obj", Justification = "Using the same names as in the .Net framework.")]
        public static bool ReferenceEquals(object objA, object objB)
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
        public static CallCollection<TFake> GetCalls<TFake>(TFake fakedObject) where TFake : class
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            return new CallCollection<TFake>(Fake.GetFakeObject(fakedObject));
        }
    }
}