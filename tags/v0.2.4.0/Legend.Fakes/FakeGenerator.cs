using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Castle.Core.Interceptor;
using System.Diagnostics;
using System.Reflection;
using Legend.Fakes.Api;
using Legend.Fakes.Configuration;

namespace Legend.Fakes
{
    internal static class FakeGenerator
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static object GenerateFake(Type type, FakeObject fakeObject)
        {
            if (type.IsInterface)
            {
                return proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[] { typeof(IFakeObjectAccessor), typeof(IHideObjectMembers) }, new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor);
            }
            else
            {
                return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakeObjectAccessor), typeof(IHideObjectMembers) }, new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor);
            }
        }

        public static object GenerateFake(Type type, FakeObject fakeObject, object[] argumentsForConstructor)
        {
            var interceptors = new IInterceptor[] {new FakeObjectAccessorInterceptor() { FakeObject = fakeObject }, fakeObject.Interceptor};
            return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakeObjectAccessor), typeof(IHideObjectMembers) }, ProxyGenerationOptions.Default, argumentsForConstructor, interceptors);
        }

        [Serializable]
        private class FakeObjectAccessorInterceptor
            : IInterceptor
        {
            public FakeObject FakeObject;

            private static readonly MethodInfo getFakeObjectMethod = typeof(IFakeObjectAccessor).GetMethod("GetFakeObject");

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
    }

    
}