using System;
using System.Runtime.Serialization;

namespace Legend.Fakes
{
    /// <summary>
    /// An exception thrown when exceptions occur regarding custom argument validations.
    /// </summary>
    [Serializable]
    public class ArgumentValidationException
        : Exception
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ArgumentValidationException() : base(ExceptionMessages.ArgumentValidationDefaultMessage)
        { }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        public ArgumentValidationException(string message)
            : base(message)
        { }

        protected ArgumentValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
