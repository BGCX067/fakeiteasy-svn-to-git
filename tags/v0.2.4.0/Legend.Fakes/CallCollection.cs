using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Legend.Fakes.Configuration;
using Legend.Fakes.Api;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes
{
    /// <summary>
    /// Contains fake calls made to a specific fake object.
    /// </summary>
    /// <typeparam name="TFake">The type of faked object.</typeparam>
    public class CallCollection<TFake>
            : IEnumerable<IFakeObjectCall> where TFake : class
    {
        internal CallCollection(FakeObject fake)
        {
            Guard.IsNotNull(fake, "fake");

            this.Fake = fake;
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
        public IEnumerable<IFakeObjectCall> Matching(Expression<Action<TFake>> callSpecification)
        {
            return this.OnMatching(callSpecification);
        }

        /// <summary>
        /// Gets all the calls in the collection that matches the specified call.
        /// </summary>
        /// <param name="callSpecification">The specification of the calls to filter out.</param>
        /// <returns>A collection of calls.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEnumerable<IFakeObjectCall> Matching<TReturn>(Expression<Func<TFake, TReturn>> callSpecification)
        {
            return this.OnMatching(callSpecification);
        }

        private IEnumerable<IFakeObjectCall> OnMatching(LambdaExpression callSpecification)
        {
            var rule = new ExpressionCallRule(callSpecification);

            return
                from call in this.Fake.RecordedCalls
                where rule.IsApplicableTo(call)
                select call;
        }

        /// <summary>
        /// Gets an enumerator enumerating the calls of the faked object.
        /// </summary>
        /// <returns>An enumerator.</returns>
        public IEnumerator<IFakeObjectCall> GetEnumerator()
        {
            return this.Fake.RecordedCalls.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
