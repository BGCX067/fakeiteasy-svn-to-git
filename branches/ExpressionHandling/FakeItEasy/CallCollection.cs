namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Contains fake calls made to a specific fake object.
    /// </summary>
    /// <typeparam name="TFake">The type of faked object.</typeparam>
    public class CallCollection<TFake>
            : IEnumerable<ICompletedFakeObjectCall>
    {
        private ExpressionCallMatcher.Factory matcherFactory;

        internal CallCollection(FakeObject fake, ExpressionCallMatcher.Factory matcherFactory)
        {
            Guard.IsNotNull(fake, "fake");
            Guard.IsNotNull(matcherFactory, "matcherFactory");

            this.Fake = fake;
            this.matcherFactory = matcherFactory;
        }

        /// <summary>
        /// Gets the faked object the calls were made to.
        /// </summary>
        public object FakedObject
        {
            get
            {
                return this.Fake.Object;
            }
        }

        /// <summary>
        /// Gets or sets the fake.
        /// </summary>
        /// <value>The fake.</value>
        internal FakeObject Fake
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets all the calls in the collection that matches the specified call.
        /// </summary>
        /// <param name="callSpecification">The specification of the calls to filter out.</param>
        /// <returns>A collection of calls.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<ICompletedFakeObjectCall> Matching(Expression<Action<TFake>> callSpecification)
        {
            return this.OnMatching(callSpecification);
        }

        /// <summary>
        /// Gets all the calls in the collection that matches the specified call.
        /// </summary>
        /// <param name="callSpecification">The specification of the calls to filter out.</param>
        /// <returns>A collection of calls.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<ICompletedFakeObjectCall> Matching<TReturn>(Expression<Func<TFake, TReturn>> callSpecification)
        {
            return this.OnMatching(callSpecification);
        }

        private IEnumerable<ICompletedFakeObjectCall> OnMatching(LambdaExpression callSpecification)
        {
            var matcher = this.matcherFactory.Invoke(callSpecification);
            
            return
                from call in this.Fake.RecordedCallsInScope
                where matcher.Matches(call)
                select call;
        }

        /// <summary>
        /// Gets an enumerator enumerating the calls of the faked object.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<ICompletedFakeObjectCall> GetEnumerator()
        {
            return this.Fake.RecordedCallsInScope.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
