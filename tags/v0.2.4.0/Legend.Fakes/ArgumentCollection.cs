using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Legend.Fakes
{
    public class ArgumentCollection
        : IEnumerable<object>
    {
        #region Fields
        private readonly object[] arguments;
        #endregion

        #region Construction
        [DebuggerStepThrough]
        public ArgumentCollection(object[] arguments, IEnumerable<string> argumentNames)
        {
            Guard.IsNotNull(arguments, "arguments");
            Guard.IsNotNull(argumentNames, "argumentNames");

            if (arguments.Length != argumentNames.Count())
            {
                throw new ArgumentException(ExceptionMessages.WrongNumberOfArgumentNamesMessage, "argumentNames");
            }

            this.arguments = arguments;
            this.ArgumentNames = argumentNames;
        }

        [DebuggerStepThrough]
        public ArgumentCollection(object[] arguments, MethodInfo method)
            : this(arguments, GetArgumentNames(method))
        {

        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of arguments in the list.
        /// </summary>
        public int Count
        {
            [DebuggerStepThrough]
            get
            {
                return this.arguments.Length;
            }
        }

        /// <summary>
        /// Gets an empty ArgumentList.
        /// </summary>
        public static ArgumentCollection Empty
        {
            [DebuggerStepThrough]
            get
            {
                return new ArgumentCollection(new object[] { }, new string[] { });
            }
        }

        /// <summary>
        /// Gets the names of the arguments in the list.
        /// </summary>
        public IEnumerable<string> ArgumentNames { get; private set; }

        /// <summary>
        /// Gets the argument at the specified index.
        /// </summary>
        /// <param name="argumentIndex">The index of the argument to get.</param>
        /// <returns>The argument at the specified index.</returns>
        public object this[int argumentIndex]
        {
            [DebuggerStepThrough]
            get
            {
                return this.arguments[argumentIndex];
            }
        }
        #endregion

        #region Methods
        [DebuggerStepThrough]
        private static IEnumerable<string> GetArgumentNames(MethodInfo method)
        {
            Guard.IsNotNull(method, "method");

            return method.GetParameters().Select(x => x.Name);
        }

        /// <summary>
        /// Gets an enumerator that enumerates the arguments in the list.
        /// </summary>
        /// <returns>An enumerator.</returns>
        [DebuggerStepThrough]
        public IEnumerator<object> GetEnumerator()
        {
            foreach (object argument in this.arguments)
            {
                yield return argument;
            }
        }

        [DebuggerStepThrough]
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.arguments.GetEnumerator();
        }

        /// <summary>
        /// Gets the argument at the specified index.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="index">The index of the argument.</param>
        /// <returns>The argument at the specified index.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T Get<T>(int index)
        {
            return (T)this.arguments[index];
        }

        /// <summary>
        /// Gets the argument with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the argument to get.</typeparam>
        /// <param name="argumentName">The name of the argument.</param>
        /// <returns>The argument with the specified name.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T Get<T>(string argumentName)
        {
            var index = GetArgumentIndex(argumentName);

            return (T)this.arguments[index];
        }

        private int GetArgumentIndex(string argumentName)
        {
            int index = 0;

            foreach (var name in this.ArgumentNames)
            {
                if (name.Equals(argumentName, StringComparison.Ordinal))
                {
                    return index;
                }

                index++;
            }

            throw new ArgumentException(ExceptionMessages.ArgumentNameDoesNotExist, "argumentName");
        }
        #endregion
    }
}