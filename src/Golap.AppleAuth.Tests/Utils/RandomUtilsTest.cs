using System;
using System.Linq;
using FluentAssertions;
using Golap.AppleAuth.Utils;
using Xunit;

namespace Golap.AppleAuth.Tests.Utils
{
    public class RandomUtilsTest
    {
        [Fact]
        public void CreateHexString_ReturnRandomValue()
        {
            var pool = Enumerable.Range(0, 15).Select(e => RandomUtils.CreateHexString(15)).ToArray();

            pool.Should().OnlyHaveUniqueItems();
            pool.Select(e => e.Length).Should().AllBeEquivalentTo(44);
        }

        [Fact]
        public void CreateHexString_InvalidParams_ThrowArgumentException()
        {
            FluentActions.Invoking(() => RandomUtils.CreateHexString(-1)).Should().Throw<ArgumentException>();
        }
    }
}
