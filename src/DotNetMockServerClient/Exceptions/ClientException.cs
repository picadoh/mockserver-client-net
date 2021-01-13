// -----------------------------------------------------------------------
// <copyright file="ClientException.cs" company="Calrom Ltd.">
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
    public sealed class ClientException : Exception
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        public ClientException()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The state store last exception.</param>
        public ClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The state store last exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private ClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Left empty on purpose
        }
    }
}
