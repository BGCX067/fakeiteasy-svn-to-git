using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Legend.Fakes.Api
{
    /// <summary>
    /// Represents a call to a fake object at interception time.
    /// </summary>
    public interface IWritableFakeObjectCall
        : IFakeObjectCall
    {
        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="value">The return value to set.</param>
        void SetReturnValue(object value);
    }
}
