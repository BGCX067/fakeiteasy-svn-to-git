using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Legend.Fakes.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes.Assertion
{
    public interface IFakeAssertions<TFake>
            : IHideObjectMembers
    {

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled(Expression<Action<TFake>> voidCall);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasNotCalled(Expression<Action<TFake>> voidCall);

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        void WasNotCalled<TMember>(Expression<Func<TFake, TMember>> returnValueCall);
    }
}
