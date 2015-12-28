using FakeItEasy.Expressions;
using System;
namespace FakeItEasy
{
    /// <summary>
    /// Provides validation extension to the ArgumentValidations{T} class.
    /// </summary>
    public static class ArgumentValidationsExtensions
    {
        /// <summary>
        /// Validates that the specified argument is not null.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="validations">The validations to extend.</param>
        /// <returns>A validator.</returns>
        public static ArgumentValidator<T> IsNotNull<T>(this ArgumentValidations<T> validations) where T : class
        {
            return ArgumentValidator<T>.Create(x => x != null, "Not NULL");
        }
    }
}
