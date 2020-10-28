using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class SequenceMatcherTests
    {
        private readonly SequenceMatcher matcher = new SequenceMatcher();

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("1")]
        public void DoesntMatchVeryEmptyOrOneLengthSequences(string password)
        {
            var res = matcher.MatchPassword(password);
            res.Should().BeEmpty();
        }

        [Theory]
        [InlineData("", "!")]
        [InlineData("", "22")]
        [InlineData("!", "!")]
        [InlineData("!", "22")]
        [InlineData("22", "!")]
        [InlineData("22", "22")]
        public void MatchesEmbeddedSequencePatterns(string prefix, string suffix)
        {
            const string pattern = "jihg";

            var password = prefix + pattern + suffix;
            var i = prefix.Length;
            var j = i + pattern.Length - 1;

            var result = matcher.MatchPassword(password).OfType<SequenceMatch>().ToList();

            var expected = new[]
            {
                new SequenceMatch
                {
                    Ascending = false,
                    i = i,
                    j = j,
                    SequenceName = "lower",
                    SequenceSpace = 26,
                    Token = pattern,
                },
            };
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("ABC", "upper", true, 26)]
        [InlineData("CBA", "upper", false, 26)]
        [InlineData("PQR", "upper", true, 26)]
        [InlineData("RQP", "upper", false, 26)]
        [InlineData("XYZ", "upper", true, 26)]
        [InlineData("ZYX", "upper", false, 26)]
        [InlineData("abcd", "lower", true, 26)]
        [InlineData("dcba", "lower", false, 26)]
        [InlineData("jihg", "lower", false, 26)]
        [InlineData("wxyz", "lower", true, 26)]
        [InlineData("zxvt", "lower", false, 26)]
        [InlineData("0369", "digits", true, 10)]
        [InlineData("97531", "digits", false, 10)]
        public void MatchesGeneralSequence(string password, string name, bool ascending, int space)
        {
            var result = matcher.MatchPassword(password).OfType<SequenceMatch>().ToList();

            var expected = new[]
            {
                new SequenceMatch
                {
                    Ascending = ascending,
                    i = 0,
                    j = password.Length - 1,
                    SequenceName = name,
                    SequenceSpace = space,
                    Token = password,
                },
            };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesOverlappingPatterns()
        {
            var result = matcher.MatchPassword("abcbabc").OfType<SequenceMatch>().ToList();

            var expected = new[]
            {
                new SequenceMatch
                {
                    Ascending = true,
                    i = 0,
                    j = 2,
                    SequenceName = "lower",
                    SequenceSpace = 26,
                    Token = "abc",
                },
                new SequenceMatch
                {
                    Ascending = false,
                    i = 2,
                    j = 4,
                    SequenceName = "lower",
                    SequenceSpace = 26,
                    Token = "cba",
                },
                new SequenceMatch
                {
                    Ascending = true,
                    i = 4,
                    j = 6,
                    SequenceName = "lower",
                    SequenceSpace = 26,
                    Token = "abc",
                },
            };
            result.Should().BeEquivalentTo(expected);
        }
    }
}
