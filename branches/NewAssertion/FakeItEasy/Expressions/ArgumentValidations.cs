namespace FakeItEasy.Expressions
{
    using System.Linq.Expressions;
    using FakeItEasy.Configuration;
    using System;

    public abstract class ArgumentValidations<T>
        : IHideObjectMembers
    {
        internal abstract bool IsValid(T argument);

        /// <summary>
        /// Reverse the is valid of the validator that comes after the not, so that
        /// if the validator is valid the result is false and if the validator is not
        /// valid the result is true.
        /// </summary>
        public ArgumentValidations<T> Not
        {
            get
            {
                return new NotValidations<T>(this);
            }
        }

        public virtual ArgumentValidator<T> Matches(Func<T, bool> predicate)
        {
            return ArgumentValidator<T>.Create(this, predicate, "Predicate");
        }

        /// <summary>
        /// Gets an argument validator that validates that the argument is
        /// of the specified type or any derivative.
        /// </summary>
        /// <typeparam name="TType">The type to check for.</typeparam>
        /// <returns>An argument validator.</returns>
        public virtual ArgumentValidator<T> IsInstanceOf<TType>()
        {
            return ArgumentValidator<T>.Create(this, x => x is TType, "Instance of {0}".FormatInvariant(typeof(TType)));
        }

        /// <summary>
        /// The base implementation returns the empty string.
        /// </summary>
        /// <returns>Empty string.</returns>
        public override string ToString()
        {
            return string.Empty;
        }
    }
}