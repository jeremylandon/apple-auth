using System;
using FluentAssertions;
using Golap.AppleAuth.Utils;
using Xunit;

namespace Golap.AppleAuth.Tests.Utils
{
    public class Pkcs8UtilsTest
    {
        [Theory]
        [InlineData("abc", "abc")]
        [InlineData("-----BEGIN PRIVATE KEY-----abc-----END PRIVATE KEY-----", "abc")]
        [InlineData("-----BEGIN PRIVATE KEY-----\nabc\n-----END PRIVATE KEY-----", "abc")]
        public void GetBody_ReturnBody(string key, string expected)
        {
            var result = Pkcs8Utils.GetBody(key);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("\na\r\nb\r\nc", "abc")]
        [InlineData("-----BEGIN PRIVATE KEY-----\na\r\nb\r\nc\n-----END PRIVATE KEY-----", "abc")]
        public void GetBody_ReturnBodyWithoutBreakLines(string key, string expected)
        {
            var result = Pkcs8Utils.GetBody(key);

            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("    ")]
        [InlineData(null)]
        public void GetBody_InvalidParams_ThrowArgumentException(string key)
        {
            FluentActions.Invoking(() => Pkcs8Utils.GetBody(key)).Should().Throw<ArgumentException>();
        }
    }
}
