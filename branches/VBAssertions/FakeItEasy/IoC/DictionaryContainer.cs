using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FakeItEasy.Api;
using FakeItEasy.Expressions;
using System.Diagnostics;

namespace FakeItEasy.IoC
{
    /// <summary>
    /// A simple implementation of an IoC container.
    /// </summary>
    internal class DictionaryContainer
            : ServiceLocator
    {
        /// <summary>
        /// The dictionary that stores the registered services.
        /// </summary>
        private Dictionary<Type, Func<DictionaryContainer, object>> registeredServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryContainer"/> class.
        /// </summary>
        public DictionaryContainer()
        {
            this.registeredServices = new Dictionary<Type, Func<DictionaryContainer, object>>();
        }

        /// <summary>
        /// Resolves an instance of the specified component type.
        /// </summary>
        /// <param name="componentType">Type of the component.</param>
        /// <returns>An instance of the component type.</returns>
        internal override object Resolve(Type componentType)
        {
            return this.registeredServices[componentType].Invoke(this);
        }

        /// <summary>
        /// Registers the specified resolver.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="resolver">The resolver.</param>
        internal void Register<T>(Func<DictionaryContainer, T> resolver)
        {
            this.registeredServices.Add(typeof(T), c => resolver.Invoke(c));
        }

        /// <summary>
        /// Registers the specified resolver as a singleton.
        /// </summary>
        /// <typeparam name="T">The type of component to register.</typeparam>
        /// <param name="resolver">The resolver.</param>
        internal void RegisterSingleton<T>(Func<DictionaryContainer, T> resolver)
        {
            var handle = new object();
            Func<DictionaryContainer, T> currentResolver = c =>
            {
                lock (handle)
                {
                    var singletonInstance = resolver.Invoke(c);
                    currentResolver = x => singletonInstance;
                    return singletonInstance;
                }
            };

            Register<T>(c => currentResolver.Invoke(c));
        }
    }
}