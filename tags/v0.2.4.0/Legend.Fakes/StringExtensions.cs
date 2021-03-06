﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Legend.Fakes
{
    /// <summary>
    /// Provides extension methods for the <see cref="System.String" /> class.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Replaces the format item in a specified System.String with the text equivalent
        /// of the value of a corresponding System.Object instance in a specified array using
        /// invariant culture as <see cref="IFormatProvider" />.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arguments">An <see cref="Object" /> array containing zero or more objects to format.</param>
        /// <returns>The formatted string.</returns>
        public static string FormatInvariant(this string format, params object[] arguments)
        {
            return string.Format(CultureInfo.InvariantCulture, format, arguments);
        }
    }

}
