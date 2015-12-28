namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using System;

    public class ArgumentValidations<T>
        : IHideObjectMembers
    {
        public ArgumentValidator<T> Matches(Func<T, bool> predicate)
        {
            return ArgumentValidator<T>.Create(predicate, "Predicate");
        }

        /// <summary>
        /// Gets an argument validator that validates that the argument is
        /// of the specified type or any derivative.
        /// </summary>
        /// <typeparam name="TType">The type to check for.</typeparam>
        /// <returns>An argument validator.</returns>
        public ArgumentValidator<T> IsInstanceOf<TType>()
        {
            return ArgumentValidator<T>.Create(x => x is TType, "Instance of {0}".FormatInvariant(typeof(TType)));
        }
    }
}