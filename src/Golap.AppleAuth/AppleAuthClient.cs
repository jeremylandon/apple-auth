using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Dawn;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Exceptions;
using Golap.AppleAuth.Utils;

namespace Golap.AppleAuth
{
    public class AppleAuthClient : IAppleAuthClient, IDisposable
    {
        private const string AppleAuthTokenEndpoint = "https://appleid.apple.com/auth/token";
        private const string AppleAuthAuthorizeEndpoint = "https://appleid.apple.com/auth/authorize";
        private readonly IAppleTokenGenerator _tokenGenerator;
        private readonly AppleAuthSetting _authSetting;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenGenerator"/> class.
        /// </summary>
        /// <param name="authSetting">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="appleKeySetting">Informations of the key created for the "Sign in with Apple" feature</param>
        public AppleAuthClient(AppleAuthSetting authSetting, AppleKeySetting appleKeySetting) : this(authSetting, new AppleTokenGenerator(authSetting.TeamId, authSetting.ClientId, appleKeySetting), new HttpClient()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenGenerator"/> class.
        /// </summary>
        /// <param name="authSetting">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="appleKeySetting">Informations of the key created for the "Sign in with Apple" feature</param>
        /// <param name="httpClient"></param>
        public AppleAuthClient(AppleAuthSetting authSetting, AppleKeySetting appleKeySetting, HttpClient httpClient) : this(authSetting, new AppleTokenGenerator(authSetting.TeamId, authSetting.ClientId, appleKeySetting), httpClient) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenGenerator"/> class.
        /// </summary>
        /// <param name="authSetting">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="tokenGenerator">Generator of the jwt tokens</param>
        public AppleAuthClient(AppleAuthSetting authSetting, IAppleTokenGenerator tokenGenerator) : this(authSetting, tokenGenerator, new HttpClient()) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="AppleTokenGenerator"/> class.
        /// </summary>
        /// <param name="authSetting">Your subscription identifier present in the membership section in your apple developer account</param>
        /// <param name="tokenGenerator">Generator of the jwt tokens</param>
        /// <param name="httpClient"></param>
        public AppleAuthClient(AppleAuthSetting authSetting, IAppleTokenGenerator tokenGenerator, HttpClient httpClient)
        {
            _authSetting = Guard.Argument(authSetting, nameof(authSetting)).NotNull();
            _tokenGenerator = Guard.Argument(tokenGenerator, nameof(tokenGenerator)).NotNull().Value;
            _httpClient = Guard.Argument(httpClient, nameof(httpClient)).NotNull();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }

        public Uri CreateLoginUri()
        {
            var queryParams = new NameValueCollection
                {
                    ["response_type"] = "code id_token",
                    ["client_id"] = _authSetting.ClientId,
                    ["redirect_uri"] = _authSetting.RedirectUri,
                    ["state"] = RandomUtils.CreateHexString(15),
                    ["scope"] = _authSetting.Scope,
                    ["response_mode"] = "form_post"
                };
            var uriBuilder = new UriBuilder(AppleAuthAuthorizeEndpoint)
                {
                    Query = string.Join("&", (
                        from key in queryParams.AllKeys
                        from value in queryParams.GetValues(key)
                        select $"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value)}"
                    ).ToArray())
                };

            return uriBuilder.Uri;
        }

        public Task<AppleAccessToken> GetAccessTokenAsync(string code)
        {
            var body = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("redirect_uri", this._authSetting.RedirectUri),
                    new KeyValuePair<string, string>("client_id", this._authSetting.ClientId),
                    new KeyValuePair<string, string>("client_secret",_tokenGenerator.Generate()),
                };

            return InternalPostAuthTokenRequestAsync(body);
        }

        public Task<AppleAccessToken> GetRefreshTokenAsync(string refreshToken)
        {
            var body = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refreshToken),
                    new KeyValuePair<string, string>("redirect_uri", _authSetting.RedirectUri),
                    new KeyValuePair<string, string>("client_id", _authSetting.ClientId),
                    new KeyValuePair<string, string>("client_secret",_tokenGenerator.Generate()),
                };

            return InternalPostAuthTokenRequestAsync(body);
        }

        private async Task<AppleAccessToken> InternalPostAuthTokenRequestAsync(IEnumerable<KeyValuePair<string, string>> requestBody)
        {
            var response = await _httpClient.PostAsync(AppleAuthTokenEndpoint, new FormUrlEncodedContent(requestBody));
            if (!response.IsSuccessStatusCode)
            {
                throw new AppleAuthException(await response.Content.ReadAsStringAsync());
            }

            return await JsonSerializer.DeserializeAsync<AppleAccessToken>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
