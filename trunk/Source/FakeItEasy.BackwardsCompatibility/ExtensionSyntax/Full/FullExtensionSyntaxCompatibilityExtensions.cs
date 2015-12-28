﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Assertion;

namespace FakeItEasy.ExtensionSyntax.Full
{
    public static class FullExtensionSyntaxCompatibilityExtensions
    {
        /// <summary>
        /// Gets an object that provides assertions for the specified fake object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake object.</typeparam>
        /// <param name="fakedObject">The fake object to get assertions for.</param>
        /// <returns>An assertion object.</returns>
        /// <exception cref="ArgumentException">The object passed in is not a faked object.</exception>
        [Obsolete]
        public static IFakeAssertions<TFake> Assert<TFake>(this TFake fakedObject) where TFake : class
        {
            Guard.IsNotNull(fakedObject, "fakedObject");

            return Fake.Assert(fakedObject);
        }
    }
}
