using System.Text.RegularExpressions;
using Dawn;

namespace Golap.AppleAuth.Utils
{
    /// <summary>
    /// Utility class for loading keys.
    /// </summary>
    internal static class Pkcs8Utils
    {
        private const string Pkcs8PemHeader = "-----BEGIN PRIVATE KEY-----";
        private const string Pkcs8PemFooter = "-----END PRIVATE KEY-----";

        /// <summary>
        /// Get base64 content of key without break lines
        /// </summary>
        public static string GetBody(string key)
        {
            Guard.Argument(key, nameof(key)).NotNull().NotWhiteSpace();

            var body = key.Replace(Pkcs8PemHeader, "").Replace(Pkcs8PemFooter, "").Trim();

            return Regex.Replace(body, @"\t|\n|\r", "");
        }
    }
}
