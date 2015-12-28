namespace FakeItEasy
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides methods for guarding method arguments.
    /// </summary>
    internal static class Guard
    {
        /// <summary>
        /// Determines whether the specified argument is null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <exception cref="ArgumentNullException">The specified argument was null.</exception>
        [DebuggerStepThrough]
        public static void IsNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Determines whether the specified argument is in the given range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <param name="argumentName">Name of the argument.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified argument was not in the given range.</exception>
        [DebuggerStepThrough]
        public static void IsInRange<T>(T argument, T lowerBound, T upperBound, string argumentName) where T : IComparable<T>
        {
            if (argument.CompareTo(lowerBound) < 0 || argument.CompareTo(upperBound) > 0)
            {
                throw new ArgumentOutOfRangeException(argumentName);
            }
        }
    }
}
