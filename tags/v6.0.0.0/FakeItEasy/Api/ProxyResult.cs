namespace FakeItEasy.Api
{
    using System;
    using System.Diagnostics;

    [Serializable]
    public abstract class ProxyResult
    {
        private IFakedProxy proxyField;
        
        protected ProxyResult(Type proxiedType)
        {
            Guard.IsNotNull(proxiedType, "proxiedType");

            this.ProxiedType = proxiedType;
        }

        public Type ProxiedType
        {
            get;
            private set;
        }

        public IFakedProxy Proxy
        {
            [DebuggerStepThrough]
            get
            {
                return this.proxyField;
            }
            [DebuggerStepThrough]
            protected set
            {
                Guard.IsNotNull(value, "value");

                if (!this.ProxiedType.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException("The specified proxy is not of the correct type.");
                }

                this.proxyField = value;
            }
        }

        public abstract event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;
    }
}
