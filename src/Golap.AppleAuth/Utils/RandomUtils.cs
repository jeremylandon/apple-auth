using System;
using System.Security.Cryptography;
using Dawn;

namespace Golap.AppleAuth.Utils
{
    /// <summary>
    /// Utility class to generate random values
    /// </summary>
    internal static class RandomUtils
    {
        internal static string CreateHexString(int hexCount)
        {
            Guard.Argument(hexCount, nameof(hexCount)).NotNegative();

            byte[] bytes = new byte[hexCount];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }

            return BitConverter.ToString(bytes);
        }
    }
}
