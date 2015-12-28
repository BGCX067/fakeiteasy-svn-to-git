using System;
using System.Collections.Generic;
using FakeItEasy.Api;
using System.Linq.Expressions;
namespace FakeItEasy
{
    /// <summary>
    /// Contains fake calls made to a specific fake object.
    /// </summary>
    /// <typeparam name="TFake">The type of faked object.</typeparam>
    public interface ICallCollection<TFake>
        : IEnumerable<ICompletedFakeObjectCall>
    {
        /// <summary>
        /// Gets the faked object the calls were made to.
        /// </summary>
        TFake FakedObject { get; }

        /// <summary>
        /// Gets all the calls in the collection that matches the specified call.
        /// </summary>
        /// <param name="callSpecification">The specification of the calls to filter out.</param>
        /// <returns>A collection of calls.</returns>
        IEnumerable<ICompletedFakeObjectCall> Matching(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Gets all the calls in the collection that matches the specified call.
        /// </summary>
        /// <param name="callSpecification">The specification of the calls to filter out.</param>
        /// <returns>A collection of calls.</returns>
        IEnumerable<ICompletedFakeObjectCall> Matching<TReturn>(Expression<Func<TFake, TReturn>> callSpecification);
    }
}
