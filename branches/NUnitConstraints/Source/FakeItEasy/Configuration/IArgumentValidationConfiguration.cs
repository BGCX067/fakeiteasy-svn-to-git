using FakeItEasy.Api;
using System;

namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Provides configurations to validate arguments of a fake object call.
    /// </summary>
    /// <typeparam name="TInterface">The type of interface to return.</typeparam>
    public interface IArgumentValidationConfiguration<TInterface>
        : IHideObjectMembers
    {
        /// <summary>
        /// Configures the call to be accepted when the specified predicate returns true.
        /// </summary>
        /// <param name="argumentsPredicate">The argument predicate.</param>
        /// <returns>A configuration object.</returns>
        TInterface WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
