namespace FakeItEasy.VisualBasic
{
    using System;
    using System.Linq.Expressions;
    using FakeItEasy.Api;
    using FakeItEasy.Configuration;
    /// <summary>
    /// Configurations for visual basic where a predicate can be set for the arguments
    /// of a call.
    /// </summary>
    /// <typeparam name="TFake">The type of faked object.</typeparam>
    public interface IVisualBasicArgumentsConfiguration<TFake>
        : IVisualBasicConfiguration<TFake>
    {
        /// <summary>
        /// Allows you to specify a predicate that validates if the arguments of a call.
        /// </summary>
        /// <param name="argumentsPredicate">The predicate that validates the arguments.</param>
        /// <returns>A configuration object.</returns>
        IVisualBasicConfiguration<TFake> WhenArgumentsMatch(Func<ArgumentCollection, bool> argumentsPredicate);
    }
}
