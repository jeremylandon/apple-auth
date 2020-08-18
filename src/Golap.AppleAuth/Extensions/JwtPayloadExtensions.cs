using System.IdentityModel.Tokens.Jwt;
using Golap.AppleAuth.Entities;

namespace Golap.AppleAuth.Extensions
{
    public static class JwtPayloadExtensions
    {
        /// <summary>
        /// Helper class to retreive more easily all claims used by Apple in the <see cref="JwtPayload"/>
        /// </summary>
        public static AppleJwtTokenPayload ToAppleJwtPayload(this JwtPayload jwtPayload) => new AppleJwtTokenPayload(jwtPayload.Claims);
    }
}
