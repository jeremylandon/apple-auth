using System.Threading.Tasks;
using Golap.AppleAuth.Entities;

namespace Golap.AppleAuth
{
    public interface IAppleAuthClient
    {
        /// <summary>
        /// Get the access token from the server based on the grant code returned by Apple
        /// </summary>
        Task<AppleAccessToken> GetAccessTokenAsync(string code);
        /// <summary>
        /// Get the access token from the server based on the refresh token returned by Apple
        /// </summary>
        Task<AppleAccessToken> GetRefreshTokenAsync(string refreshToken);
    }
}
