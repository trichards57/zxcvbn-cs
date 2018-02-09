using System.Linq;
using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class SequenceMatcherTests
    {
        private readonly SequenceMatcher _matcher = new SequenceMatcher();

        [Theory, InlineData(""), InlineData("a"), InlineData("1")]
        public void DoesntMatchShortSequences(string password)
        {
            var res = _matcher.MatchPassword(password);
            res.Should().BeEmpty();
        }

        [Fact]
        public void MatchesOverlappingPatterns()
        {
            var res = _matcher.MatchPassword("abcbabc").OfType<SequenceMatch>().ToList();

            res.Should().HaveCount(3);

            res[0].Pattern.Should().Be("sequence");
            res[0].Token.Should().Be("abc");
            res[0].i.Should().Be(0);
            res[0].j.Should().Be(2);
            res[0].Ascending.Should().Be(true);

            res[1].Pattern.Should().Be("sequence");
            res[1].Token.Should().Be("cba");
            res[1].i.Should().Be(2);
            res[1].j.Should().Be(4);
            res[1].Ascending.Should().Be(false);

            res[2].Pattern.Should().Be("sequence");
            res[2].Token.Should().Be("abc");
            res[2].i.Should().Be(4);
            res[2].j.Should().Be(6);
            res[2].Ascending.Should().Be(true);
        }
    }
}