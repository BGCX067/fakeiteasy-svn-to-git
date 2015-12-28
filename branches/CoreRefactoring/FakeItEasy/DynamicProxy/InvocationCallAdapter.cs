namespace FakeItEasy.DynamicProxy
{
    using System.Diagnostics;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using FakeItEasy.Api;
    using System;

    /// <summary>
    /// An adapter that adapts an <see cref="IInvocation" /> to a <see cref="IFakeObjectCall" />.
    /// </summary>
    internal class InvocationCallAdapter
        : IWritableFakeObjectCall, ICompletedFakeObjectCall
    {
        private IInvocation Invocation;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationCallAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        [DebuggerStepThrough]
        public InvocationCallAdapter(IInvocation invocation)
        {
            this.Invocation = invocation;
            this.Arguments = new ArgumentCollection(invocation.Arguments, invocation.Method);
        }


        /// <summary>
        /// The method that's called.
        /// </summary>
        public MethodInfo Method
        {
            [DebuggerStepThrough]
            get
            {
                return this.Invocation.Method;
            }
        }

        /// <summary>
        /// Sets the return value of the call.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        [DebuggerStepThrough]
        public void SetReturnValue(object returnValue)
        {
            this.Invocation.ReturnValue = returnValue;
        }

        /// <summary>
        /// The arguments used in the call.
        /// </summary>
        public ArgumentCollection Arguments
        {
            get;
            private set;
        }

        /// <summary>
        /// The value set to be returned from the call.
        /// </summary>
        public object ReturnValue
        {
            get { return this.Invocation.ReturnValue; }
        }

        /// <summary>
        /// Returns a description of the call.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.GetDescription();
        }

        /// <summary>
        /// The faked object the call is performed on.
        /// </summary>
        /// <value></value>
        public object FakedObject
        {
            get { return this.Invocation.Proxy; }
        }


        /// <summary>
        /// Freezes the call so that it can no longer be modified.
        /// </summary>
        /// <returns>A completed fake object call.</returns>
        public ICompletedFakeObjectCall Freeze()
        {
            return this;
        }

        public void CallBaseMethod()
        {
            this.Invocation.Proceed();
        }


        public void SetArgumentValue(int index, object value)
        {
            this.Invocation.SetArgumentValue(index, value);
        }
    }
}
