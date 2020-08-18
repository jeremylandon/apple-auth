using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Extensions;
using Xunit;

namespace Golap.AppleAuth.Tests.Entities
{
    public class AppleJwtTokenPayloadTest
    {
        [Fact]
        public void This_MapAllClaims()
        {
            var jwtPayload = new JwtPayload(new[]
                {
                    new Claim("email", "test@privaterelay.appleid.com"),
                    new Claim("nonce_supported", "true"),
                    new Claim("email_verified", "true"),
                    new Claim("is_private_email", "true"),
                    new Claim("real_user_status", "2"),
                    new Claim("iss", "abc"),
                });

            var appleJwtPayload = jwtPayload.ToAppleJwtPayload();

            appleJwtPayload.Email.Should().Be("test@privaterelay.appleid.com");
            appleJwtPayload.EmailVerified.Should().BeTrue();
            appleJwtPayload.IsPrivateEmail.Should().BeTrue();
            appleJwtPayload.RealUserStatus.Should().Be(AppleRealUserStatus.LikelyReal);
            appleJwtPayload.NonceSupported.Should().Be(true);
            appleJwtPayload.Iss.Should().Be("abc");
        }

        [Theory]
        [InlineData("test@privaterelaY.appleid.com", true)]
        [InlineData("test@test.com", false)]
        public void This_WithoutIsPrivateEmailClaim_CanDeduceValue(string email, bool result)
        {
            var jwtPayload = new JwtPayload(new[]
                {
                    new Claim("email", email),
                });

            var appleJwtPayload = jwtPayload.ToAppleJwtPayload();

            appleJwtPayload.IsPrivateEmail.Should().Be(result);
        }

        [Fact]
        public void This_WithoutClaims_ReturnDefaultValues()
        {
            var jwtPayload = new JwtPayload(new Claim[0]);

            var appleJwtPayload = jwtPayload.ToAppleJwtPayload();

            appleJwtPayload.Email.Should().BeNull();
            appleJwtPayload.EmailVerified.Should().BeNull();
            appleJwtPayload.IsPrivateEmail.Should().BeNull();
            appleJwtPayload.RealUserStatus.Should().BeNull();
            appleJwtPayload.NonceSupported.Should().BeNull();
        }
    }
}
