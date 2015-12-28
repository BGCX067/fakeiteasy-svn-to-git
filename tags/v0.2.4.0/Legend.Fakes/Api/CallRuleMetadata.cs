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

namespace Legend.Fakes.Api
{
    /// <summary>
    /// Keeps track of metadata for interceptions.
    /// </summary>
    [Serializable]
    internal class CallRuleMetadata
    {
        public IFakeObjectCallRule Rule;
        public int CalledNumberOfTimes;

        public bool HasNotBeenCalledSpecifiedNumberOfTimes()
        {
            return this.Rule.NumberOfTimesToCall == null || this.CalledNumberOfTimes < this.Rule.NumberOfTimesToCall.Value;
        }
    }
}
