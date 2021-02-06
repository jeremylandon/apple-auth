using System;

namespace Golap.AppleAuth.Exceptions
{
    /// <summary>
    /// Exception in the AppleAuth framework
    /// </summary>
    [Serializable]
    public class AppleAuthException : Exception
    {
        /// <inheritdoc />
        public AppleAuthException()
        {
        }

        /// <inheritdoc />
        public AppleAuthException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        public AppleAuthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
