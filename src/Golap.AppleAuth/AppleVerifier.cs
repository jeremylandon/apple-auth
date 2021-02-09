using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dawn;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth
{
    /// <summary>
    /// An implementation of <see cref="IAppleVerifier"/> to validate JWT Tokens send by apple during authentication
    /// </summary>
    /// <summary>
    /// Validates the JWT Tokens send by apple during authentication
    /// </summary>
    public class AppleVerifier : IAppleVerifier, IDisposable
    {
        private const string ApplePublicKeysEndpoint = "https://appleid.apple.com/auth/keys";
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleVerifier"/> class.
        /// </summary>
        public AppleVerifier() : this(new HttpClient()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleVerifier"/> class.
        /// </summary>
        public AppleVerifier(HttpClient httpClient)
        {
            _httpClient = Guard.Argument(httpClient, nameof(httpClient)).NotNull();
        }

        /// <inheritdoc/>
        public Task<SecurityToken> ValidateAsync(string token)
        {
            return ValidateAsync(token, null);
        }

        /// <inheritdoc/>
        public async Task<SecurityToken> ValidateAsync(string token, string clientId)
        {
            Guard.Argument(token, nameof(token)).NotNull().NotWhiteSpace().Matches(@"^[A-Za-z0-9-_=]+\.[A-Za-z0-9-_=]+\.?[A-Za-z0-9-_.+/=]*$");

            var response = await _httpClient.GetAsync(ApplePublicKeysEndpoint);
            if (!response.IsSuccessStatusCode)
            {
                throw new AppleAuthException(await response.Content.ReadAsStringAsync());
            }

            var applePublicKeys = await JsonSerializer.DeserializeAsync<AppleKeysResponse>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var kid = new JwtSecurityTokenHandler().ReadJwtToken(token).Header.Kid;
            var publicKey = applePublicKeys?.Keys?.FirstOrDefault(key => key.Kid == kid);
            if (publicKey == null)
            {
                throw new AppleAuthException($"kid {kid} not found in apple public keys");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = publicKey,
                ValidIssuer = AppleJwtSettings.Issuer,
                ValidAudience = clientId,
                ValidateAudience = clientId != null
            };

            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            return validatedToken;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
