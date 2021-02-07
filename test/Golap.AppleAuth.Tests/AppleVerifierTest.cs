using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Exceptions;
using Golap.AppleAuth.Tests.Core;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace Golap.AppleAuth.Tests
{
    public class AppleVerifierTest
    {
        private readonly JwtValidation _settings;

        public AppleVerifierTest()
        {
            _settings = TestSettingsHelper.GetTestSettings<TestSettings>().JwtValidation;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("...")]
        public async Task ValidateAsync_NotJwt_ThrowArgumentException(string token)
        {
            await FluentActions.Invoking(() => new AppleVerifier().ValidateAsync(token, "test")).Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task ValidateAsync_Valid_ReturnSecurityToken()
        {
            var client = CreateValidFakeAppleHttpClient();
            var verifier = new AppleVerifier(client);

            var result = await verifier.ValidateAsync(_settings.ValidJwtToken, _settings.Audience);

            result?.ToString().Should().Be(_settings.ValidAppleResponse);
        }

        [Fact]
        public async Task ValidateAsync_Valid_CanBypassAudienceValidation()
        {
            var client = CreateValidFakeAppleHttpClient();
            var verifier = new AppleVerifier(client);

            var result = await verifier.ValidateAsync(_settings.ValidJwtToken);

            result?.ToString().Should().Be(_settings.ValidAppleResponse);
        }

        [Fact]
        public async Task ValidateAsync_InvalidAudience_ThrowSecurityTokenInvalidAudienceException()
        {
            var client = CreateValidFakeAppleHttpClient();
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(_settings.ValidJwtToken, "badAud")).Should().ThrowAsync<SecurityTokenInvalidAudienceException>();
        }

        [Fact]
        public async Task ValidateAsync_NotValidSignature_ThrowSecurityTokenInvalidSignatureException()
        {
            var jwtToken = _settings.InvalidJwtToken;
            var client = CreateValidFakeAppleHttpClient();
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken, _settings.Audience)).Should().ThrowAsync<SecurityTokenInvalidSignatureException>();
        }

        [Fact]
        public async Task ValidateAsync_AppleDoenstReturnKid_ThrowAppleAuthException()
        {
            var jwtToken = _settings.InvalidJwtToken;
            var appleResponse = new AppleKeysResponse();
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(appleResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
            });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken, _settings.Audience)).Should().ThrowAsync<AppleAuthException>();
        }

        [Fact]
        public async Task ValidateAsync_AppleReturnError_ThrowAppleAuthException()
        {
            var responseData = "error";
            var jwtToken = _settings.InvalidJwtToken;
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(responseData) });
            var client = new HttpClient(handlerStub);
            var verifier = new AppleVerifier(client);

            await FluentActions.Invoking(() => verifier.ValidateAsync(jwtToken, _settings.Audience)).Should().ThrowAsync<AppleAuthException>().WithMessage(responseData);
        }

        #region Internal

        private HttpClient CreateValidFakeAppleHttpClient()
        {
            var appleResponse = new AppleKeysResponse { Keys = new[] { new JsonWebKey(_settings.JsonKey) } };
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(appleResponse, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
                });
            return new HttpClient(handlerStub);
        }

        #endregion
    }
}
