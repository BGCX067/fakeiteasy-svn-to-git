using System;
using Castle.Core.Interceptor;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace FakeItEasy.Api
{
    internal class InvocationCallAdapter
        : IWritableFakeObjectCall, ICompletedFakeObjectCall
    {
        private IInvocation Invocation;

        [DebuggerStepThrough]
        public InvocationCallAdapter(IInvocation invocation)
        {
            this.Invocation = invocation;
            this.Arguments = new ArgumentCollection(invocation.Arguments, invocation.Method);
        }


        public MethodInfo Method
        {
            [DebuggerStepThrough]
            get
            {
                return this.Invocation.Method;
            }
        }

        [DebuggerStepThrough]
        public void SetReturnValue(object returnValue)
        {
            this.Invocation.ReturnValue = returnValue;
        }

        public ArgumentCollection Arguments
        {
            get;
            private set;
        }

        public object ReturnValue
        {
            get { return this.Invocation.ReturnValue; }
        }

        public override string ToString()
        {
            return this.GetDescription();
        }

        public object FakedObject
        {
            get { return this.Invocation.Proxy; }
        }


        public ICompletedFakeObjectCall Freeze()
        {
            return this;
        }


        public void CallBaseMethod()
        {
            this.Invocation.Proceed();
        }
    }
}
