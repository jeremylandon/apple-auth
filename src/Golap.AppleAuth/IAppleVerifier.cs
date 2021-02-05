using System.Threading.Tasks;
using Golap.AppleAuth.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth
{
    /// <summary>
    /// Validates the JWT Tokens send by apple during authentication
    /// </summary>
    public interface IAppleVerifier
    {
        /// <summary>
        /// Check the token validity. Throw <see cref="AppleAuthException"/> if the token is invalid
        /// </summary>
        /// <returns>The recognized <see cref="SecurityToken"/></returns>
        /// <exception cref="AppleAuthException"></exception>
        Task<SecurityToken> ValidateAsync(string token, string clientId);
    }
}
