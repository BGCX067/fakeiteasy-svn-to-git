﻿namespace FakeItEasy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Provides extension methods for the common uses.
    /// </summary>
    internal static class CommonExtensions
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

        /// <summary>
        /// Gets an enumerable of tuples where the first value of each tuple is a value
        /// from the first collection and the second value of each tuple is the value at the same postion
        /// from the second collection.
        /// </summary>
        /// <typeparam name="TFirst">The type of values in the first collection.</typeparam>
        /// <typeparam name="TSecond">The type of values in the second collection.</typeparam>
        /// <param name="firstCollection">The first of the collections to combine.</param>
        /// <param name="secondCollection">The second of the collections to combine.</param>
        /// <returns>An enumerable of tuples.</returns>
        public static IEnumerable<Tuple<TFirst, TSecond>> Pairwise<TFirst, TSecond>(this IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
        {
            Guard.IsNotNull(firstCollection, "firstCollection");
            Guard.IsNotNull(secondCollection, "secondCollection");

            return new PairwiseEnumerable<TFirst, TSecond>(firstCollection, secondCollection);
        }

        /// <summary>
        /// Gets a value indicating if the specified collection is empty.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection.</typeparam>
        /// <param name="collection">The collection to test.</param>
        /// <returns>True if the collection is empty.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            Guard.IsNotNull(collection, "collection");

            using (var enumerator = collection.GetEnumerator())
            {
                return !enumerator.MoveNext();
            }
        }

        private class PairwiseEnumerable<TFirst, TSecond>
            : IEnumerable<Tuple<TFirst, TSecond>>
        {
            private IEnumerable<TFirst> firstCollection;
            private IEnumerable<TSecond> secondCollection;

            public PairwiseEnumerable(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
            {
                this.firstCollection = firstCollection;
                this.secondCollection = secondCollection;
            }

            public IEnumerator<Tuple<TFirst, TSecond>> GetEnumerator()
            {
                return new PairwiseEnumerator<TFirst, TSecond>(this.firstCollection, this.secondCollection);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private class PairwiseEnumerator<TFirst, TSecond>
                : IEnumerator<Tuple<TFirst, TSecond>>
            {
                private IEnumerable<TFirst> firstCollection;
                private IEnumerable<TSecond> secondCollection;
                private IEnumerator<TFirst> firsts;
                private IEnumerator<TSecond> seconds;


                public PairwiseEnumerator(IEnumerable<TFirst> firstCollection, IEnumerable<TSecond> secondCollection)
                {
                    this.firstCollection = firstCollection;
                    this.secondCollection = secondCollection;
                    this.Reset();
                }

                public Tuple<TFirst, TSecond> Current
                {
                    get { return new Tuple<TFirst, TSecond>(firsts.Current, seconds.Current); }
                }

                public void Dispose()
                {
                    this.firsts.Dispose();
                    this.seconds.Dispose();
                    GC.SuppressFinalize(this);
                }

                object System.Collections.IEnumerator.Current
                {
                    get { return this.Current; }
                }

                public bool MoveNext()
                {
                    var firstsCanMove = this.firsts.MoveNext();
                    var secondsCanMove = this.seconds.MoveNext();

                    if (firstsCanMove != secondsCanMove)
                    {
                        throw new InvalidOperationException("The second collection is not the same size as the first collection.");
                    }

                    return firstsCanMove && secondsCanMove;
                }

                public void Reset()
                {
                    this.firsts = firstCollection.GetEnumerator();
                    this.seconds = secondCollection.GetEnumerator();
                }
            }
        }
    }
}