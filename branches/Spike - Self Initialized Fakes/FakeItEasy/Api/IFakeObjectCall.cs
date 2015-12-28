using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FakeItEasy.Api
{

    /// <summary>
    /// Represents a call to a fake object.
    /// </summary>
    public interface IFakeObjectCall
    {
        /// <summary>
        /// The method that's called.
        /// </summary>
        MethodInfo Method { get; }

        /// <summary>
        /// The arguments used in the call.
        /// </summary>
        ArgumentCollection Arguments { get; }

        /// <summary>
        /// The faked object the call is performed on.
        /// </summary>
        object FakedObject { get; }
    }
}