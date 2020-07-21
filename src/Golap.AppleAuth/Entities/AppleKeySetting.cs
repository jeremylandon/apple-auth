using Dawn;
using Golap.AppleAuth.Utils;

namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// Informations of the key created for the "Sign in with Apple" feature
    /// </summary>
    public class AppleKeySetting
    {
        /// <summary>
        /// The identifier for the private key on the Apple
        /// </summary>
        public string KeyId { get; }
        /// <summary>
        /// The contents of service key
        /// </summary>
        public string PrivateKey { get; }

        /// <summary>
        /// Get base64 content of key
        /// </summary>
        public string PrivateKeyBody => Pkcs8Utils.GetBody(PrivateKey);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleKeySetting"/> class.
        /// </summary>
        /// <param name="keyId">Key you created for Sign in with Apple</param>
        /// <param name="privateKey">The contents of service key</param>
        public AppleKeySetting(string keyId, string privateKey)
        {
            Guard.Argument(keyId, nameof(keyId)).NotNull().NotWhiteSpace();
            Guard.Argument(privateKey, nameof(privateKey)).NotNull().NotWhiteSpace();

            KeyId = keyId;
            PrivateKey = privateKey;
        }
    }
}
