using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoBogus;
using FluentAssertions;
using Golap.AppleAuth.Entities;
using Golap.AppleAuth.Exceptions;
using Golap.AppleAuth.Tests.Core;
using Moq;
using Xunit;

namespace Golap.AppleAuth.Tests
{
    public class AppleAuthClientTest
    {
        #region data

        public class ConstructorBadArgsData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                var a = new AppleAuthSetting("a", "b", "https://apple.com");
                var b = new AppleTokenGenerator("a", "b", new AppleKeySetting("a", "b"));
                var c = new HttpClient();
                yield return new object[] { a, b, null };
                yield return new object[] { a, null, c };
                yield return new object[] { null, b, c };
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion

        private readonly Mock<IAppleTokenGenerator> _appleTokenGeneratorMock;

        public AppleAuthClientTest()
        {
            _appleTokenGeneratorMock = new Mock<IAppleTokenGenerator>();
        }

        [Theory]
        [ClassData(typeof(ConstructorBadArgsData))]
        public void AppleAuthClient_NotValidArgument_ThrowArgumentException(AppleAuthSetting authSetting, IAppleTokenGenerator privateKeySetting, HttpClient httpClient)
        {
            FluentActions.Invoking(() => new AppleAuthClient(authSetting, privateKeySetting, httpClient)).Should().Throw<ArgumentException>();
        }

        [Fact]
        public async Task ValidateAsync_AppleDoenstReturnKid_AppleAuthException()
        {
            _appleTokenGeneratorMock.Setup(e => e.Generate(It.IsAny<TimeSpan>())).Returns("abc");
            var response = AutoFaker.Generate<AppleAccessToken>();
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })),
            });
            var client = GetClient(handlerStub);

            var result = await client.GetAccessTokenAsync("abc");

            result.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetAccessTokenAsync_AppleReturnError_ThrowAppleAuthException()
        {
            var responseData = "error";
            var handlerStub = new DelegatingHandlerStub(new HttpResponseMessage() { StatusCode = HttpStatusCode.BadRequest, Content = new StringContent(responseData) });
            var client = GetClient(handlerStub);

            await FluentActions.Invoking(() => client.GetAccessTokenAsync("abc")).Should().ThrowAsync<AppleAuthException>().WithMessage(responseData);
        }

        private AppleAuthClient GetClient(DelegatingHandler handler)
        {
            return new AppleAuthClient(AutoFaker.Generate<AppleAuthSetting>(), _appleTokenGeneratorMock.Object, new HttpClient(handler));
        }
    }
}
