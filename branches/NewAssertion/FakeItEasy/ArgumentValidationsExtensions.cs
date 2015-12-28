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
        /// Validates that an argument is null.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="validations">The parent validations.</param>
        /// <returns>An argument validator.</returns>
        public static ArgumentValidator<T> IsNull<T>(this ArgumentValidations<T> validations) where T : class
        {
            return ArgumentValidator<T>.Create(validations, x => x == null, "NULL");
        }
    }
}
