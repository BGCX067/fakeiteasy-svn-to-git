namespace FakeItEasy.VisualBasic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Provides extensions for the Visual basic configuration.
    /// </summary>
    public static class VisualBasicHelpers
    {
        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object</returns>
        public static IVoidConfiguration<TFake> WithAnyArguments<TFake>(this IVisualBasicConfiguration<TFake> configuration)
        {
            return configuration.WhenArgumentsMatch(x => true);
        }
    }
}
