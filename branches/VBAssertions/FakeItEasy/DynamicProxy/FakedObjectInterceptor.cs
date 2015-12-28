namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using FakeItEasy.Api;

    /// <summary>
    /// An interceptor that gets the fake object the proxy belongs to.
    /// </summary>
    [Serializable]
    internal class FakedObjectInterceptor
        : IInterceptor
    {
        internal FakeObject FakeObject;

        private static readonly MethodInfo getFakeObjectMethod = typeof(IFakedProxy).GetMethod("GetFakeObject");

        [DebuggerStepThrough]
        void IInterceptor.Intercept(IInvocation invocation)
        {
            if (getFakeObjectMethod.Equals(invocation.Method))
            {
                invocation.ReturnValue = this.FakeObject;
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
