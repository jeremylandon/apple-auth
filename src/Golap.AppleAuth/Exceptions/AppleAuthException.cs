using System;

namespace Golap.AppleAuth.Exceptions
{
    [Serializable]
    public class AppleAuthException : Exception
    {
        public AppleAuthException()
        {
        }

        public AppleAuthException(string message)
            : base(message)
        {
        }

        public AppleAuthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
