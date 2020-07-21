using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Golap.AppleAuth.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth
{
    /// <summary>
    /// Creates JWT tokens based on Apple informations
    /// </summary>
    public class AppleTokenGenerator : IAppleTokenGenerator
    {
        private readonly string _teamId;
        private readonly string _clientId;
        private readonly AppleKeySetting _setting;
        private readonly TimeSpan _defaultExpireInInterval = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenGenerator"/> class.
        /// </summary>
        /// <param name="teamId">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="clientId">The service's identfier or bundle id of your application</param>
        /// <param name="setting">Informations of the key that you are created for "Sign in with Apple"</param>
        public AppleTokenGenerator(string teamId, string clientId, AppleKeySetting setting)
        {
            _teamId = teamId;
            _clientId = clientId;
            _setting = setting;
        }

        public string Generate()
        {
            return Generate(_defaultExpireInInterval);
        }

        public string Generate(TimeSpan expireInInterval)
        {
            using var cngKey = CngKey.Import(Convert.FromBase64String(_setting.PrivateKeyBody), CngKeyBlobFormat.Pkcs8PrivateBlob);
            using var algorithm = new ECDsaCng(cngKey) { HashAlgorithm = CngAlgorithm.ECDsaP256 };
            var signingCredentials = new SigningCredentials(new ECDsaSecurityKey(algorithm), SecurityAlgorithms.EcdsaSha256)
            {
                // prevent ObjectDisposedException exception when this method it's called multiple times
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var now = DateTime.UtcNow;
            var exp = now.Add(expireInInterval);
            var token = new JwtSecurityToken(
                claims: new List<Claim>
                    {
                           new Claim("iss", _teamId),
                           new Claim("sub", _clientId),
                           new Claim("aud", "https://appleid.apple.com"),
                           new Claim("iat", EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),
                           new Claim("exp", EpochTime.GetIntDate(exp).ToString(), ClaimValueTypes.Integer64),
                    },
                signingCredentials: signingCredentials);
            token.Header.Add("kid", _setting.KeyId);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
