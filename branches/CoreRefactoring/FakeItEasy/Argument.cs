namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using FakeItEasy.Extensibility;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Static class that allows you to validate specific arguments of a fake object call.
    /// </summary>
    public static class Argument
    {
        /// <summary>
        /// Provides an extension poin for argument validation extension methods.
        /// </summary>
        public static IExtensibleIs Is 
        {
            get
            {
                return null;
            }
        }
    }
}