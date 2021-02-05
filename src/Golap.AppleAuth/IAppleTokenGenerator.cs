using System;

namespace Golap.AppleAuth
{
    /// <summary>
    /// Create JWT tokens based on Apple informations
    /// </summary>
    public interface IAppleTokenGenerator
    {
        /// <summary>
        /// Generates a JWT token that expires in 10 minutes
        /// </summary>
        string Generate();

        /// <summary>
        /// Generates a JWT token
        /// </summary>
        string Generate(TimeSpan expireInInterval);
    }
}
