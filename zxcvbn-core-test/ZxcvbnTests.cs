using FluentAssertions;
using Xunit;

namespace Zxcvbn.Tests
{
    public class ZxcvbnTests
    {
        [Fact]
        public void EmptyPassword()
        {
            Zxcvbn.MatchPassword("").Should().Be(0);
        }
    }
}