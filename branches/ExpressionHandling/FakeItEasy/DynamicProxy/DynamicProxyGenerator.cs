namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Linq;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Api;

    internal class DynamicProxyGenerator
            : IFakeProxyGenerator
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        internal interface ICanInterceptObjectMembers
        {
            string ToString();
            
            int GetHashCode();
            
            bool Equals(object obj);
        }

        public bool CanCreateProxyOfType(Type type)
        {
            if (type.IsSealed)
            {
                return false;
            }

            if (!type.IsAbstract && !type.IsInterface)
            {
                var accessibleConstructors =
                    from constructor in type.GetConstructors()
                    where !constructor.IsPrivate
                    select constructor;

                if (accessibleConstructors.FirstOrDefault() == null)
                {
                    return false;
                }
            }

            return true;
        }

        public IFakedProxy GenerateFake(FakeObject fakeObject, Action<IWritableFakeObjectCall> interceptionCallback, Type type, params object[] argumentsForConstructor)
        {
            var interceptor = new FakeObjectCallInterceptor(interceptionCallback);

            if (argumentsForConstructor.Length == 0)
            {
                return (IFakedProxy)DynamicProxyGenerator.GenerateProxyWithoutArgumentsForConstructor(type, fakeObject, interceptor);
            }
            else
            {
                return (IFakedProxy)DynamicProxyGenerator.GenerateProxyWithArgumentsForConstructor(type, fakeObject, interceptor, argumentsForConstructor);
            }
        }

        private static object GenerateProxyWithoutArgumentsForConstructor(Type type, FakeObject fakeObject, IInterceptor interceptor)
        {
            if (type.IsInterface)
            {
                return proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) }, new FakedObjectInterceptor() { FakeObject = fakeObject }, interceptor);
            }
            else
            {
                return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) }, new FakedObjectInterceptor() { FakeObject = fakeObject }, interceptor);
            }
        }

        private static object GenerateProxyWithArgumentsForConstructor(Type type, FakeObject fakeObject, IInterceptor interceptor, object[] argumentsForConstructor)
        {
            var interceptors = new IInterceptor[] { new FakedObjectInterceptor() { FakeObject = fakeObject }, interceptor };
            return proxyGenerator.CreateClassProxy(type, new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) }, ProxyGenerationOptions.Default, argumentsForConstructor, interceptors);
        }
    }
}