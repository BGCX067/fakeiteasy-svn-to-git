using System;
using Castle.Core.Interceptor;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace FakeItEasy.Api
{
    /// <summary>
    /// An interceptor that intercepts calls in dynamic proxy and
    /// delegates them to the fake object.
    /// </summary>
    [Serializable]
    internal class FakeObjectInterceptor
        : IInterceptor
    {
        private FakeObject fake;

        [DebuggerStepThrough]
        public FakeObjectInterceptor(FakeObject fake)
        {
            this.fake = fake;
        }

        [DebuggerStepThrough]
        void IInterceptor.Intercept(IInvocation invocation)
        {
            this.fake.Intercept(new InvocationCallAdapter(invocation));
        }

        private class InvocationCallAdapter
            : IWritableFakeObjectCall
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
                get { return this.Invocation.Proxy;  }
            }
        }
    }
}
