using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using FakeItEasy.Api;

namespace FakeItEasy.Configuration
{
    public interface ICallbackConfiguration<TInterface>
    {
        /// <summary>
        /// Executes the specified action when a matching call is being made.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        /// <returns>A configuration object.</returns>
        TInterface Invokes(Action<IFakeObjectCall> action);
    }
}
