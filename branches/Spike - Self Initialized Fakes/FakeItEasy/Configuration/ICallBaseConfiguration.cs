using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.ComponentModel;
using FakeItEasy.Api;

namespace FakeItEasy.Configuration
{
    public interface ICallBaseConfiguration<TFake>
            : IHideObjectMembers
    {
        /// <summary>
        /// When the configured method or methods are called the call
        /// will be delegated to the base method of the faked method.
        /// </summary>
        /// <returns>A configuration object.</returns>
        /// <exception cref="InvalidOperationException">The fake object is of an abstract type or an interface
        /// and no base method exists.</exception>
        IAfterCallSpecifiedConfiguration<TFake> CallsBaseMethod();
    }
}
