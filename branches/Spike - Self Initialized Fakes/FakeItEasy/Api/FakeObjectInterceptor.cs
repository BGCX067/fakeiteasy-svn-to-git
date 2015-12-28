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
    }
}