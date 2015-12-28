using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using FakeItEasy.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace FakeItEasy.Assertion
{
    public interface IFakeAssertions<TFake>
            : IHideObjectMembers
    {
        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <param name="voidCall"></param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled(Expression<Action<TFake>> voidCall);

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="voidCall">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled(Expression<Action<TFake>> voidCall, Expression<Func<int, bool>> repeatValidation);

        /// <summary>
        /// Throws an exception if the specified call has not been called.
        /// </summary>
        /// <typeparam name="TMember">The type of return values from the function that is asserted upon.</typeparam>
        /// <param name="returnValueCall">An expression describing the call to assert that has been called.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall);

        /// <summary>
        /// Asserts that the specified call was called the number of times that is validated by the
        /// repeatValidation predicate passed to the method.
        /// </summary>
        /// <param name="voidCall">The call to assert on.</param>
        /// <param name="repeatValidation">A lambda predicate validating that will be passed the number of times
        /// the specified call was invoked and returns true for a valid repeat.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall, Expression<Func<int, bool>> repeatValidation);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasNotCalled(Expression<Action<TFake>> voidCall);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasNotCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall);
    }
}
