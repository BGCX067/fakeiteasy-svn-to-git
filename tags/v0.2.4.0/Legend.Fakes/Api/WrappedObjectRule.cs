using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Globalization;
using System.Reflection;
using Legend.Fakes.Configuration;
using System.Diagnostics;
using System.ComponentModel;

namespace Legend.Fakes.Api
{
    /// <summary>
    /// A call rule that applies to any call and just delegates the
    /// call to the wrapped object.
    /// </summary>
    public class WrappedObjectRule
        : IFakeObjectCallRule
    {
        private object wrappedObject;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="wrappedInstance">The object to wrap.</param>
        public WrappedObjectRule(object wrappedInstance)
        {
            this.wrappedObject = wrappedInstance;
        }

        public bool IsApplicableTo(IFakeObjectCall fakeObjectCall)
        {
            return true;
        }

        public void Apply(IWritableFakeObjectCall fakeObjectCall)
        {
            fakeObjectCall.SetReturnValue(fakeObjectCall.Method.Invoke(this.wrappedObject, fakeObjectCall.Arguments.ToArray()));
        }

        public int? NumberOfTimesToCall
        {
            get { return null; }
        }

    }
}