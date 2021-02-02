using System;

namespace Golap.AppleAuth
{
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
