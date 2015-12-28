namespace FakeItEasy.Expressions
{
    using System;

    /// <summary>
    /// Provides extension methods for the ArgumentValidator(T) class.
    /// </summary>
    public static class ArgumentValidatorExtensions
    {
        /// <summary>
        /// Allows you to combine the current validator with another validator, where only
        /// one of them has to be valid.
        /// </summary>
        /// <param name="validator">The validator to extend.</param>
        /// <param name="otherValidator">A delegate that returns the validator to combine with.</param>
        /// <returns>A combined validator.</returns>
        public static ArgumentValidator<T> Or<T>(this ArgumentValidator<T> validator, Func<ArgumentValidatorScope<T>, ArgumentValidator<T>> otherValidator)
        {
            Guard.IsNotNull(validator, "validator");
            Guard.IsNotNull(otherValidator, "otherValidator");

            return validator.Or(otherValidator.Invoke(A<T>.That));
        }
    }
}
