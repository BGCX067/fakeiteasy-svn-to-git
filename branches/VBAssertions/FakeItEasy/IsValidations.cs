namespace FakeItEasy
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FakeItEasy.Extensibility;
    using FakeItEasy.Expressions.ArgumentValidators;
    using FakeItEasy.Expressions;

    /// <summary>
    /// Provides extension methods for validating arguments.
    /// </summary>
    public static class IsValidations
    {
        /// <summary>
        /// Specifies that any value of the specified type is valid.
        /// </summary>
        /// <typeparam name="T">The type of arguments that are valid.</typeparam>
        /// <param name="extensionPoint">The extension point.</param>
        /// <returns>Does not return anything, should only be used inside an callSpecification-expression.</returns>
        [ArgumentValidator(typeof(AnyValidator<>))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T Any<T>(this IExtensibleIs extensionPoint)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Specifies that arguments that are matched by the specified predicate are valid.
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="extensionPoint">The extension point.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>Does not return anything, should only be used inside an callSpecification-expression.</returns>
        [ArgumentValidator(typeof(PredicateValidator<>))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "predicate", Justification="It is used but only through reflection.")]
        public static T Matching<T>(this IExtensibleIs extensionPoint, Func<T, bool> predicate)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Specifies that any argument that is not null is valid.
        /// </summary>
        /// <typeparam name="T">The type of the argument</typeparam>
        /// <param name="extensionPoint">The extension point.</param>
        /// <returns>
        /// Does not return anything, should only be used inside an callSpecification-expression.
        /// </returns>
        [ArgumentValidator(typeof(NotNullValidator))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "extensionPoint")]
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static T NotNull<T>(this IExtensibleIs extensionPoint)
        {
            throw new NotSupportedException();
        }
    }
}
