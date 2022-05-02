using System;

namespace ObjectAssertion
{
    /// <summary>
    /// Thrown when an assertion failed.
    /// </summary>
    [Serializable]
    public class ObjectAssertionException : Exception
    {
        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        public ObjectAssertionException(string message) : base(message)
        {
        }

        /// <param name="message">The error message that explains
        /// the reason for the exception</param>
        /// <param name="inner">The exception that caused the
        /// current exception</param>
        public ObjectAssertionException(string message, Exception inner) :
            base(message, inner)
        {
        }

        /// <summary>
        /// Serialization Constructor
        /// </summary>
        protected ObjectAssertionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
        }
    }
}