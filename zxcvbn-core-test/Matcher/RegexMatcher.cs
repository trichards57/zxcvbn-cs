using System.Linq;
using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;

namespace Zxcvbn.Tests.Matcher
{
    public class RegexMatcherTests
    {
        [Fact]
        public void MatchesMidString()
        {
            var re = new RegexMatcher("\\d{3,}", 10);

            var res = re.MatchPassword("abc123def").ToList();
            res.Count.Should().Be(1);

            res[0].i.Should().Be(3);
            res[0].j.Should().Be(5);
            res[0].Token.Should().Be("123");
        }

        [Fact]
        public void MatchesMultiple()
        {
            var re = new RegexMatcher("\\d{3,}", 10);

            var res = re.MatchPassword("123456789a12345b1234567").ToList();
            res.Count.Should().Be(3);

            res[2].i.Should().Be(16);
            res[2].j.Should().Be(23);
            res[2].Token.Should().Be("1234567");
        }

        [Fact]
        public void MatchesNoMatch()
        {
            var re = new RegexMatcher("\\d{3,}", 10);

            var res = re.MatchPassword("12");
            res.Should().BeEmpty();

            res = re.MatchPassword("dfsdfdfhgjkdfngjl");
            res.Should().BeEmpty();
        }
    }
}