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
    public class FakeObjectCallEventArgs : EventArgs
    {
        public FakeObjectCallEventArgs(IFakeObjectCall fakeObjectCall)
        {
            this.FakeObjectCall = fakeObjectCall;
        }

        public IFakeObjectCall FakeObjectCall { get; private set; }
    }
}
