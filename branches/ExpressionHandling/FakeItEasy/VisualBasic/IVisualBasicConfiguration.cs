namespace FakeItEasy.VisualBasic
{
    using System;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;

    /// <summary>
    /// Configurations for visual basic.
    /// </summary>
    /// <typeparam name="TFake">The type of the configured faked object.</typeparam>
    public interface IVisualBasicConfiguration<TFake>
            : IVoidConfiguration<TFake>
    {
        /// <summary>
        /// Allows you to specify a predicate that validates if the arguments of a call.
        /// </summary>
        /// <param name="argumentsPredicate">The predicate that validates the arguments.</param>
        /// <returns>A configuration object.</returns>
        IVoidConfiguration<TFake> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
