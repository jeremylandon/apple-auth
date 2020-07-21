using System.Text.Json.Serialization;

namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// The response token object returned on a successful request.
    /// </summary>
    public class AppleAccessToken
    {
        /// <summary>
        /// The access token string as issued by the authorization server.
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        /// <summary>
        /// Expiration time of the Access Token in seconds since the response was generated.
        /// </summary>
        [JsonPropertyName("expires_in")]
        public int ExpireIn { get; set; }
        /// <summary>
        /// Header, payload and signature portion.
        /// </summary>
        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
        /// <summary>
        /// Used to obtain another access token.
        /// </summary>
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// The type of token (bearer).
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
    }
}
