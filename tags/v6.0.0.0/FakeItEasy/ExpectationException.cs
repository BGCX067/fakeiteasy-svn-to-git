namespace FakeItEasy
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// An exception thrown when an expection is not met (when asserting on fake object calls).
    /// </summary>
    [Serializable]
    public class ExpectationException
        : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ExpectationException(string message)
            : base(message)
        {

        }

        protected ExpectationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
