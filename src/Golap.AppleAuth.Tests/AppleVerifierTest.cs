using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Golap.AppleAuth.Exceptions;
using Golap.AppleAuth.Models;
using Golap.AppleAuth.Tests.Core;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Golap.AppleAuth.Tests
{
    public class AppleVerifierTest
    {
        private const string JsonKey = "{'kty':'RSA','e':'AQAB','use':'sig','kid':'abc','alg':'RS256','n':'k7al_Uc5ld-2nv7Nn9V9oSeOEsxOT32wXBsUDV3aIJktZxJ58RFkX7LvFckHjFt8Du7R1jkV5jzZP2YBD6GzX5pPwAagL6t0PCvKs23bGfzxwweYf5llO483Gp7wZkpfTItIfiz0fAqOR-NKNweiJ0SaIk1hRUwCdwbUCOXFnDVa6l5MABRIKfjRmuhljSYeqnCYRZepVIaybJR8DdGTByJdgTl_1H91P_ySLCulA4B-7fVY2u_E93avbRyrOqxX6QXw1DjVIZpzPBRXmC1WlZNwbL770P-Y0IKs2Hsl791S6CIO2ax8X3LZBieLFOGYOOVVHGCzH4-Cpd0FOTUVEQ'}";

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("...")]
        public async Task ValidateAsync_NotJwt_ThrowArgumentException(string token)
        {
            await FluentActions.Invoking(() => new AppleVerifier().ValidateAsync(token)).Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ValidateAsync_Valid_ReturnSecurityToken()
        {
            #region arrange var

            var jwtToken = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6ImFiYyJ9.eyJpc3MiOiJodHRwczovL2FwcGxlaWQuYXBwbGUuY29tIiwiYXVkIjoiYS5iLmMiLCJleHAiOjIxNDc0ODM2NDcsImlhdCI6MzMzODI1MjMwLCJzdWIiOiIwMDAwMDAueHh4eC4xMTExIiwiY19oYXNoIjoieHh4eCIsImVtYWlsIjoieHh4eEBwcml2YXRlcmVsYXkuYXBwbGVpZC5jb20iLCJlbWFpbF92ZXJpZmllZCI6InRydWUiLCJpc19wcml2YXRlX2VtYWlsIjoidHJ1ZSIsImF1dGhfdGltZSI6MzMzODI1MjMwLCJub25jZV9zdXBwb3J0ZWQiOnRydWV9.Dmm0_tSmjT4DEXtMprCmP3oDoThZJqVmNh1tcsdqRpkt7-V0jqVrYFaqkiLK8W6293MJhKma-blwkscNdcw4zUQxhP1JeHrByFopQqXp-COY_lR4QjzKZU1qyIKdHkCkvODwxWP3bLDeG8GVFue3OzK8IpeRrV5ad7IZOoSFKJpXnTInRvHGx_B1XJdCLjXDgo9DrwsICL5IP1GLa1dWl0NnenaQzoijwsxaTERxSOLaA0uCIrXAq0QM5RREqBFV-uDkE-4JwF0jXVYnnmNtammOW79zZE9ylWzXdioyNcP4dbOHgi4wWfXIlJw2v8Iswx9HcbtoM-R_D__83-7lCg";
            var jsonKey = "{'kty':'RSA','e':'AQAB','use':'sig','kid':'abc','alg':'RS256','n':'k7al_Uc5ld-2nv7Nn9V9oSeOEsxOT32wXBsUDV3aIJktZxJ58RFkX7LvFckHjFt8Du7R1jkV5jzZP2YBD6GzX5pPwAagL6t0PCvKs23bGfzxwweYf5llO483Gp7wZkpfTItIfiz0fAqOR-NKNweiJ0SaIk1hRUwCdwbUCOXFnDVa6l5MABRIKfjRmuhljSYeqnCYRZepVIaybJR8DdGTByJdgTl_1H91P_ySLCulA4B-7fVY2u_E93avbRyrOqxX6QXw1DjVIZpzPBRXmC1WlZNwbL770P-Y0IKs2Hsl791S6CIO2ax8X3LZBieLFOGYOOVVHGCzH4-Cpd0FOTUVEQ'}";
            var expected = "{\"alg\":\"RS256\",\"typ\":\"JWT\",\"kid\":\"abc\"}.{\"iss\":\"https://appleid.apple.com\",\"aud\":\"a.b.c\",\"exp\":2147483647,\"iat\":333825230,\"sub\":\"000000.xxxx.1111\",\"c_hash\":\"xxxx\",\"email\":\"xxxx@privaterelay.appleid.com\",\"email_verified\":\"true\",\"is_private_email\":\"true\",\"auth_time\":333825230,\"nonce_supported\":true}";

            #endregion

            var appleResponse = new AppleKeys { Keys = new[] { new JsonWebKey(jsonKey) } };
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(appleResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
            });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);
            var result = await verifier.ValidateAsync(jwtToken);

            result.Should().NotBeNull();
            result.ToString().Should().Be(expected);
        }

        [Fact]
        public async Task ValidateAsync_NotValidSignature_ThrowSecurityTokenInvalidSignatureException()
        {
            var jwtToken = GetInvalidJwtToken();
            var appleResponse = new AppleKeys { Keys = new[] { new JsonWebKey(JsonKey) } };
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(appleResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
            });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken)).Should().ThrowAsync<SecurityTokenInvalidSignatureException>();
        }

        [Fact]
        public async Task ValidateAsync_AppleDoenstReturnKid_AppleAuthException()
        {
            var jwtToken = GetInvalidJwtToken();
            var appleResponse = new AppleKeys();
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(appleResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
            });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken)).Should().ThrowAsync<AppleAuthException>();
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("error", "error")]
        public async Task ValidateAsync_AppleReturnError_AppleAuthException(string responseData, string messageException)
        {
            var jwtToken = GetInvalidJwtToken();
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = responseData == null ? null : new StringContent(responseData) });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken)).Should().ThrowAsync<AppleAuthException>().WithMessage(messageException);
        }

        private string GetInvalidJwtToken() =>
            "eyJhbGciOiJSUzI1NiIsImtpZCI6ImFiYyJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwiaWF0IjoxNTE2MjM5MDIyfQ.Mukojqt_KA8GmaBMKzThl1-PNdV0cs4LDVHRgJKoILYHVq69bmVBkpwItCy-DY_DlPY4SbDSrEyDfKamkHVS6H_J1EGxUpamLPDY6UKDy7RkgRhISBn6h9T-kx0fPSMd6JnS2IyGdkrqgFiwORXqF7iYssHfan5ues4AiJ_wDUcjjkbms4TlG6zayS9eN4z15ILwWvhEZNuq1GselllSIgNW_JJoRRVyrs4WGj4hRI4VCBkp4krxYit0jzNF-V96bFJF7jtdBoZhKkmZhQAkJN6PR42B5l-tntOoblUz8dvpTNzL8ya1QG1KbfeFmW3J_iXQdi0E1pTBSqKw-PBrXA";
    }
}
