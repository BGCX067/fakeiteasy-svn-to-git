using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Legend
{
    internal static class Guard
    {
        [DebuggerStepThrough]
        public static void IsNotNull(object argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

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
