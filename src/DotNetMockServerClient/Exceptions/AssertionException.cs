// -----------------------------------------------------------------------
// <copyright file="AssertionException.cs" company="Calrom Ltd.">
// Under MIT license
// </copyright>
// -----------------------------------------------------------------------

namespace DotNetMockServerClient
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// AssertionException class.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public sealed class AssertionException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AssertionException"/> class.
        /// </summary>
        public AssertionException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AssertionException"/> class.
        /// </summary>
        /// <param name="message">The state store last exception.</param>
        public AssertionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="AssertionException"/> class.
        /// </summary>
        /// <param name="message">The state store last exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public AssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private AssertionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Left empty on purpose
        }
    }
}
