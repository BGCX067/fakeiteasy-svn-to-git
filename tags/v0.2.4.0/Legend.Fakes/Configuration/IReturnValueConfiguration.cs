using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using Legend.Fakes.Api;

namespace Legend.Fakes.Configuration
{
    public interface IReturnValueConfiguration<TFake, TMember>
        : IExceptionThrowerConfiguration<TFake>, IHideObjectMembers
    {
        /// <summary>
        /// Specifies the value to return when the configured call is made.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TFake> Returns(TMember value);

        /// <summary>
        /// Specifies a function used to produce a return value when the configured call is made.
        /// The function will be called each time this call is made and can return different values
        /// each time.
        /// </summary>
        /// <param name="valueProducer">A function that produces the return value.</param>
        /// <returns>A configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TFake> Returns(Func<TMember> valueProducer);

        IAfterCallSpecifiedConfiguration<TFake> Returns(Func<IFakeObjectCall, TMember> valueProducer);
    }
}
