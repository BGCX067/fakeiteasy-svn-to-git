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
    /// Represents a completed call to a fake object.
    /// </summary>
    public interface ICompletedFakeObjectCall
        : IFakeObjectCall
    {
        /// <summary>
        /// The value set to be returned from the call.
        /// </summary>
        object ReturnValue { get; }
    }
}
