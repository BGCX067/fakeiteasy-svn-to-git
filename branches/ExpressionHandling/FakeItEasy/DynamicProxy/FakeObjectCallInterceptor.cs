namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Diagnostics;
    using Castle.Core.Interceptor;
    using FakeItEasy.Api;

    /// <summary>
    /// An interceptor that intercepts calls in dynamic proxy and
    /// delegates them to the fake object.
    /// </summary>
    [Serializable]
    internal class FakeObjectCallInterceptor
        : IInterceptor
    {
        private Action<IWritableFakeObjectCall> interceptionCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeObjectInterceptor"/> class.
        /// </summary>
        /// <param name="fake">The fake.</param>
        [DebuggerStepThrough]
        public FakeObjectCallInterceptor(Action<IWritableFakeObjectCall> interceptionCallback)
        {
            this.interceptionCallback = interceptionCallback;
        }

        [DebuggerStepThrough]
        void IInterceptor.Intercept(IInvocation invocation)
        {
            this.interceptionCallback(new InvocationCallAdapter(invocation));
        }
    }
}