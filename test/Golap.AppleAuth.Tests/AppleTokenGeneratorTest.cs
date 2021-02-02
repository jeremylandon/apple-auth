using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Golap.AppleAuth.Entities;
using Xunit;

namespace Golap.AppleAuth.Tests
{
    public class AppleTokenGeneratorTest
    {
        private string _teamId = "123456789X";
        private string _clientId = "com.golap.app";
        private string _keyId = "X987654321";
        private string _privateKey;

        [Fact]
        public async Task Generate_CreateJwtToken_WithMandatoriesClaims()
        {
            _privateKey = await GetValidKeyP8ContentAsync();
            var client = CreateClient();

            var token = client.Generate();

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwtToken.Issuer.Should().Be(_teamId);
            jwtToken.Subject.Should().Be(_clientId);
            jwtToken.Audiences.Should().HaveCount(1).And.Contain(AppleJwtSettings.Audience);
        }

        [Fact]
        public async Task Generate_CreateJwtToken_ExpireIn10MinByDefault()
        {
            _privateKey = await GetValidKeyP8ContentAsync();
            var client = CreateClient();

            var token = client.Generate();

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwtToken.ValidTo.Should().Be(jwtToken.IssuedAt.AddMinutes(10));
        }


        [Theory]
        [MemberData(nameof(GenerateExpirationTestData))]
        public async Task Generate_CreateJwtToken_WithRightExpiration(TimeSpan exp)
        {
            _privateKey = await GetValidKeyP8ContentAsync();
            var client = CreateClient();

            var token = client.Generate(exp);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwtToken.ValidTo.Should().Be(jwtToken.IssuedAt.Add(exp));
        }

        /// <summary>
        /// prevent <see cref="ObjectDisposedException"/>
        /// </summary>
        [Fact]
        public async Task Generate_CanBeRunMultipleTimes()
        {
            _privateKey = await GetValidKeyP8ContentAsync();
            var client = CreateClient();

            FluentActions.Invoking(() =>
                {
                    client.Generate();
                    client.Generate();
                }).Should().NotThrow();
        }

        private Task<string> GetValidKeyP8ContentAsync()
        {
            // generated from https://8gwifi.org/
            return File.ReadAllTextAsync("Files/fake-key.p8");
        }

        private AppleTokenGenerator CreateClient(AppleKeySetting keySetting = null)
        {
            keySetting ??= new AppleKeySetting(_keyId, _privateKey);
            return new AppleTokenGenerator(_teamId, _clientId, keySetting);
        }

        #region data

        public static IEnumerable<object[]> GenerateExpirationTestData => new[]
            {
                new object[] {TimeSpan.FromDays(1)},
                new object[] { TimeSpan.FromMinutes(10) }
            };

        #endregion
    }
}
