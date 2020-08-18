using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Golap.AppleAuth.Entities
{
    /// <summary>
    /// Helper class to retreive more easily all claims used by Apple in the <see cref="JwtPayload"/>
    /// </summary>
    public class AppleJwtTokenPayload : JwtPayload
    {
        /// <summary>
        /// The email address will be either the user’s real email address or the proxy address, depending on their status private email relay service.
        /// </summary>
        public string Email => (string)this.GetValueOrDefault("email", null);
        /// <summary>
        /// Indicates whether the transaction is on a nonce-supported platform.
        /// </summary>
        public bool? NonceSupported => GetBoolClaim("nonce_supported");

        /// <summary>
        /// The value of this claim is always true, because the servers only return verified email addresses.
        /// </summary>
        public bool? EmailVerified => GetBoolClaim("email_verified");

        /// <summary>
        /// Indicates whether the email shared by the user is the proxy address.
        /// </summary>
        public bool? IsPrivateEmail => GetBoolClaim("is_private_email") ?? Email?.EndsWith("@privaterelay.appleid.com", true, CultureInfo.InvariantCulture);

        /// <summary>
        /// Indicates whether the user appears to be a real person. Use the value of this claim to mitigate fraud.
        /// </summary>
        public AppleRealUserStatus? RealUserStatus
        {
            get
            {
                var val = this.GetValueOrDefault("real_user_status", null);
                if (val == null || !Enum.TryParse<AppleRealUserStatus>(val.ToString(), out var result))
                {
                    return null;
                }

                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleJwtTokenPayload"/> class for mocking.
        /// </summary>
        protected AppleJwtTokenPayload() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppleJwtTokenPayload"/> class.
        /// </summary>
        public AppleJwtTokenPayload(IEnumerable<Claim> claims) : base(claims) { }

        private bool? GetBoolClaim(string claimType)
        {
            var val = this.GetValueOrDefault(claimType);
            if (val == null || !bool.TryParse(val.ToString(), out var result))
            {
                return null;
            }

            return result;
        }
    }
}
