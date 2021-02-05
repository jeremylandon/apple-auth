using System;
using System.Threading.Tasks;
using Golap.AppleAuth.Entities;

namespace Golap.AppleAuth
{
    /// <summary>
    /// Interact with the Apple OAuth
    /// </summary>
    public interface IAppleAuthClient
    {
        /// <summary>
        /// Generates the Login URI
        /// </summary>
        Uri GetLoginUri();

        /// <summary>
        /// Get the access token from the server based on the grant code returned by Apple
        /// </summary>
        /// <param name="code">Code received by apple after authentication (<see cref="GetLoginUri"/>)</param>
        Task<AppleAccessToken> GetAccessTokenAsync(string code);

        /// <summary>
        /// Refresh an accessToken from a refresh token
        /// </summary>
        /// <param name="refreshToken">refresh token in a previous accessToken</param>
        Task<AppleAccessToken> RefreshTokenAsync(string refreshToken);
    }
}
