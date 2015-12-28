using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes.Configuration
{
    public interface IFakeConfiguration<TFake>
        : IHideObjectMembers
    {
        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <typeparam name="TMember">The type of the return value of the member.</typeparam>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IReturnValueConfiguration<TFake, TMember> CallsTo<TMember>(Expression<Func<TFake, TMember>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object when a call that matches the specified
        /// call happens.
        /// </summary>
        /// <param name="callSpecification">An expression that specifies the calls to configure.</param>
        /// <returns>A configuration object.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IVoidConfiguration<TFake> CallsTo(Expression<Action<TFake>> callSpecification);

        /// <summary>
        /// Configures the behavior of the fake object whan a call is made to any method on the
        /// object.
        /// </summary>
        /// <returns>A configuration object.</returns>
        IVoidConfiguration<TFake> AnyCall();
    }
}