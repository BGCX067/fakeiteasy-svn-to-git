using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Diagnostics;
using System.Reflection;
using FakeItEasy.Api;
using FakeItEasy.Configuration;

namespace FakeItEasy
{
    internal static class FakeGenerator
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static object GenerateFake(Type type, FakeObject fakeObject)
        {
            if (type.IsInterface)
            {
                return proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[] { typeof(IFakedObject), typeof(ICanInterceptObjectMembers) }, new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor);
            }
            else
            {
                return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakedObject), typeof(ICanInterceptObjectMembers) }, new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor);
            }
        }

        public static object GenerateFake(Type type, FakeObject fakeObject, object[] argumentsForConstructor)
        {
            var interceptors = new IInterceptor[] {new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor};
            return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakedObject), typeof(ICanInterceptObjectMembers) }, ProxyGenerationOptions.Default, argumentsForConstructor, interceptors);
        }

        [Serializable]
        private class FakeObjectAccessorInterceptor
            : IInterceptor
        {
            public FakeObject FakeObject;

            private static readonly MethodInfo getFakeObjectMethod = typeof(IFakedObject).GetMethod("GetFakeObject");

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
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

        internal static object CreateProxy(Type type, IInterceptor interceptor)
        {
            return proxyGenerator.CreateClassProxy(type, new Type[] { }, interceptor);
        }

        internal interface ICanInterceptObjectMembers
        {
            string ToString();
            int GetHashCode();
            bool Equals(object obj);
        }
    }
}