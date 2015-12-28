using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;

namespace Legend.Fakes.Configuration
{
    public interface IExceptionThrowerConfiguration<TFake>
            : IHideObjectMembers
    {
        /// <summary>
        /// Throws the specified exception when the currently configured
        /// call gets called.
        /// </summary>
        /// <param name="exception">The exception to throw.</param>
        /// <returns>Configuration object.</returns>
        IAfterCallSpecifiedConfiguration<TFake> Throws(Exception exception);
    }
}
