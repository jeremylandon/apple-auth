using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Dawn;
using Golap.AppleAuth.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth
{
    /// <summary>
    /// An implementation of <see cref="IAppleTokenGenerator"/> to create JWT tokens based on Apple informations
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
            _teamId = Guard.Argument(teamId, nameof(teamId)).NotNull().NotWhiteSpace();
            _clientId = Guard.Argument(clientId, nameof(clientId)).NotNull().NotWhiteSpace();
            _setting = Guard.Argument(setting, nameof(setting)).NotNull();
        }

        /// <inheritdoc/>
        public string Generate()
        {
            return Generate(_defaultExpireInInterval);
        }

        /// <inheritdoc/>
        public string Generate(TimeSpan expireInInterval)
        {
            var signingFactory = new CryptoProviderFactory { CacheSignatureProviders = false };
            using var algorithm = ECDsa.Create();
            // ReSharper disable once PossibleNullReferenceException
            algorithm.ImportPkcs8PrivateKey(Convert.FromBase64String(_setting.PrivateKeyBody), out _);
            var credentials = new SigningCredentials(new ECDsaSecurityKey(algorithm)
            {
                KeyId = _setting.KeyId,
                CryptoProviderFactory = signingFactory
            }, SecurityAlgorithms.EcdsaSha256);

            var now = DateTime.UtcNow;
            var exp = now.Add(expireInInterval);
            var token = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = exp,
                IssuedAt = now,
                Issuer = _teamId,
                Audience = AppleJwtSettings.Issuer,
                Claims = new Dictionary<string, object>()
                        {
                            {"sub", _clientId}
                        }
            };

            return new JwtSecurityTokenHandler().CreateEncodedJwt(token);
        }
    }
}
