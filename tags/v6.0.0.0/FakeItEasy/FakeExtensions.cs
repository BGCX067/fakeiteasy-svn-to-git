namespace FakeItEasy
{
    using FakeItEasy.Configuration;
    using System;

    /// <summary>
    /// Provides extension methods for fake objects.
    /// </summary>
    public static class FakeExtensions
    {
        /// <summary>
        /// Specifies NumberOfTimes(1) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration to set repeat 1 to.</param>
        public static void Once<TFake>(this IRepeatConfiguration<TFake> configuration)
        {
            Guard.IsNotNull(configuration, "configuration");
            configuration.NumberOfTimes(1);
        }

        /// <summary>
        /// Specifies NumberOfTimes(2) to the IRepeatConfiguration{TFake}.
        /// </summary>
        /// <typeparam name="TFake">The type of fake object.</typeparam>
        /// <param name="configuration">The configuration to set repeat 2 to.</param>
        public static void Twice<TFake>(this IRepeatConfiguration<TFake> configuration)
        {
            Guard.IsNotNull(configuration, "configuration");

            configuration.NumberOfTimes(2);
        }

        /// <summary>
        /// Specifies that the configured call/calls should return null when called.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <typeparam name="TMember">The type of the faked member.</typeparam>
        /// <param name="configuration">The configuration to apply to.</param>
        /// <returns>A configuration object.</returns>
        public static IAfterCallSpecifiedConfiguration<TFake> ReturnsNull<TFake, TMember>(this IReturnValueConfiguration<TFake, TMember> configuration) where TMember : class
        {
            Guard.IsNotNull(configuration, "configuration");

            return configuration.Returns((TMember)null);
        }

        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object</returns>
        public static IVoidConfiguration<TFake> WithAnyArguments<TFake>(this IArgumentValidationConfiguration<IVoidConfiguration<TFake>> configuration)
        {
            return configuration.WhenArgumentsMatch(x => true);
        }

        /// <summary>
        /// Specifies that a call to the configured call should be applied no matter what arguments
        /// are used in the call to the faked object.
        /// </summary>
        /// <typeparam name="TFake">The type of the fake.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>A configuration object</returns>
        public static IReturnValueConfiguration<TFake, TMember> WithAnyArguments<TFake, TMember>(this IArgumentValidationConfiguration<IReturnValueConfiguration<TFake, TMember>> configuration)
        {
            return configuration.WhenArgumentsMatch(x => true);
        }
    }
}