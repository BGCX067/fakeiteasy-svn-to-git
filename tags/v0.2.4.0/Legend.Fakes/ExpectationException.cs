using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Legend.Fakes
{
    [Serializable]
    public class ExpectationException
        : Exception
    {
        public ExpectationException(string message)
            : base(message)
        {

        }

        protected ExpectationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
