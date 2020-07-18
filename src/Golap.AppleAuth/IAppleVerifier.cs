using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Golap.AppleAuth
{
    public interface IAppleVerifier
    {
        /// <summary>
        /// Validates a Apple-issued Json Web Token (JWT). Thrown an exception if the token can't be validated.
        /// </summary>
        Task<SecurityToken> ValidateAsync(string token);
    }
}
