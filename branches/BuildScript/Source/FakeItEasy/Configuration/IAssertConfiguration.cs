using FakeItEasy.Api;
using System;
using System.Linq.Expressions;

namespace FakeItEasy.Configuration
{
    /// <summary>
    /// Allows the developer to assert on a call that's configured.
    /// </summary>
    public interface IAssertConfiguration
        : IHideObjectMembers
    {
        /// <summary>
        /// Asserts right away that the configured must have happened.
        /// </summary>
        /// <param name="repeatPredicate">The number of times the call must have happened.</param>
        void MustHaveHappened(Expression<Func<int, bool>> repeatPredicate);
    }
}
