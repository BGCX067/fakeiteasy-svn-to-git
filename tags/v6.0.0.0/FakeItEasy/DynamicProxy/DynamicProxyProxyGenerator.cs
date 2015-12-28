namespace FakeItEasy.DynamicProxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Castle.Core.Interceptor;
    using Castle.DynamicProxy;
    using FakeItEasy.Api;
    using System.Diagnostics;

    internal class DynamicProxyProxyGenerator
         : IProxyGenerator
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static Type[] interfacesToImplement = new Type[] { typeof(IFakedProxy), typeof(ICanInterceptObjectMembers) };

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="container">A fake object container the proxy generator can use to get arguments for constructor.</param>
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        public bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IFakeObjectContainer container, out ProxyResult result)
        {
            if (typeToProxy.IsInterface)
            {
                result = GenerateInterfaceProxy(typeToProxy, fakeObject);
                return true;
            }

            var argumentsForConstructor = this.ResolveConstructorArguments(typeToProxy, container);

            if (!TypeCanBeProxiedWithArgumentsForConstructor(typeToProxy, argumentsForConstructor))
            {
                result = null;
                return false;
            }

            result = GenerateClassProxy(typeToProxy, argumentsForConstructor, fakeObject);
            return true;
        }

        /// <summary>
        /// Gets a value indicating if a proxy of the specified type can be generated and sets the generated proxy
        /// to the out parameter if it can.
        /// </summary>
        /// <param name="typeToProxy">The type to generate a proxy for.</param>
        /// <param name="fakeObject">The generated proxy must implement the IFakedProxy interface and this is the fake object
        /// that should be returned for the call to GetFakeObject().</param>
        /// <param name="argumentsForConstructor">Arguments to use for the constructor of the proxied type.</param>
        /// <param name="generatedProxy">An object containing the proxy if generation was successful.</param>
        /// <returns>True if the proxy could be generated.</returns>
        /// <exception cref="ArgumentException">The arguments in argumentsForConstructor does not match any constructor
        /// of the proxied type.</exception>
        public bool TryGenerateProxy(Type typeToProxy, FakeObject fakeObject, IEnumerable<object> argumentsForConstructor, out ProxyResult result)
        {
            if (typeToProxy.IsInterface)
            {
                throw new ArgumentException(ExceptionMessages.ArgumentsForConstructorOnInterfaceType);
            }

            result = GenerateClassProxy(typeToProxy, argumentsForConstructor, fakeObject);
            return true;
        }

        private IEnumerable<object> ResolveConstructorArguments(Type typeToProxy, IFakeObjectContainer container)
        {
            if (typeToProxy.IsInterface || TypeHasDefaultConstructor(typeToProxy))
            {
                return null;
            }

            foreach (var constructor in GetUsableConstructors(typeToProxy))
            {
                var resolvedArguments = new List<object>();

                if (ResolveArgumentsFromTypes(constructor.GetParameters().Select(x => x.ParameterType), resolvedArguments, container))
                {
                    return resolvedArguments;
                }
            }

            return null;
        }

        private static IEnumerable<ConstructorInfo> GetUsableConstructors(Type type)
        {
            return
                from constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where !constructor.IsPrivate
                select constructor;
        }

        private bool ResolveArgumentsFromTypes(IEnumerable<Type> argumentTypes, ICollection<object> arguments, IFakeObjectContainer container)
        {
            foreach (var argumentType in argumentTypes)
            {
                object resolvedArgument = null;
                if (container.TryCreateFakeObject(argumentType, null, out resolvedArgument))
                {
                    arguments.Add(resolvedArgument);
                }
                else if (TryCreateValueTypeArgument(argumentType, out resolvedArgument))
                {
                    arguments.Add(resolvedArgument);
                }
                else if (TryCreateProxiedArgument(argumentType, container, out resolvedArgument))
                {
                    arguments.Add(resolvedArgument);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryCreateProxiedArgument(Type type, IFakeObjectContainer container, out object argument)
        {
            ProxyResult result;
            if (this.TryGenerateProxy(type, null, container, out result))
            {
                argument = result.Proxy;
                return true;
            }

            argument = null;
            return false;
        }


        private static bool TryCreateValueTypeArgument(Type type, out object argument)
        {
            if (type.IsValueType)
            {
                argument = Activator.CreateInstance(type, true);
                return true;
            }

            argument = null;
            return false;
        }

        private static DynamicProxyResult GenerateClassProxy(Type typeToProxy, IEnumerable<object> argumentsForConstructor, FakeObject fakeObject)
        {
            var result = new DynamicProxyResult(typeToProxy);
            if (argumentsForConstructor == null)
            {
                result.Proxy = (IFakedProxy)proxyGenerator.CreateClassProxy(typeToProxy, interfacesToImplement, CreateFakeObjectInterceptor(fakeObject), result);
            }
            else
            {
                result.Proxy = (IFakedProxy)proxyGenerator.CreateClassProxy(typeToProxy, interfacesToImplement, new ProxyGenerationOptions(), argumentsForConstructor.ToArray(), CreateFakeObjectInterceptor(fakeObject), result);
            }
            return result;
        }

        private static bool TypeCanBeProxiedWithArgumentsForConstructor(Type typeToProxy, IEnumerable<object> argumentsForConstructor)
        {
            return TypeCanBeProxied(typeToProxy) && (argumentsForConstructor != null || TypeHasDefaultConstructor(typeToProxy));
        }

        private static DynamicProxyResult GenerateInterfaceProxy(Type typeToProxy, FakeObject fakeObject)
        {
            var result = new DynamicProxyResult(typeToProxy);
            result.Proxy = (IFakedProxy)proxyGenerator.CreateInterfaceProxyWithoutTarget(typeToProxy, interfacesToImplement, CreateFakeObjectInterceptor(fakeObject), result);
            return result;
        }

        private static bool TypeHasDefaultConstructor(Type type)
        {
            return
                (from constructor in GetUsableConstructors(type)
                 where !constructor.IsPrivate && constructor.GetParameters().Count() == 0
                 select constructor).Any();
        }

        private static bool TypeCanBeProxied(Type type)
        {
            return !type.IsSealed;
        }

        private static FakeObjectInterceptor CreateFakeObjectInterceptor(FakeObject fakeObject)
        {
            return new FakeObjectInterceptor { FakeObject = fakeObject };
        }

        [Serializable]
        private class FakeObjectInterceptor
            : IInterceptor
        {
            private static readonly MethodInfo getProxyManagerMethod = typeof(IFakedProxy).GetMethod("GetFakeObject");

            public FakeObject FakeObject;

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.Equals(getProxyManagerMethod))
                {
                    invocation.ReturnValue = this.FakeObject;
                }
                else
                {
                    invocation.Proceed();
                }
            }
        }

        [Serializable]
        private class DynamicProxyResult
            : ProxyResult, IInterceptor
        {
            public DynamicProxyResult(Type typeOfProxy)
                : base(typeOfProxy)
            { 
                
            }

            public override event EventHandler<CallInterceptedEventArgs> CallWasIntercepted;

            [DebuggerStepThrough]
            public void Intercept(IInvocation invocation)
            {
                var handler = this.CallWasIntercepted;
                if (handler != null)
                {
                    var call = new InvocationCallAdapter(invocation);
                    handler(this.Proxy, new CallInterceptedEventArgs(call));
                }
            }

            public new IFakedProxy Proxy
            {
                [DebuggerStepThrough]
                get
                {
                    return base.Proxy;
                }
                [DebuggerStepThrough]
                set
                {
                    base.Proxy = value;
                }
            }
        }

        public interface ICanInterceptObjectMembers
        {
            string ToString();
            bool Equals(object o);
            int GetHashCode();
        }
    }
}
