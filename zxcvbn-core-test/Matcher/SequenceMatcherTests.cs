using FluentAssertions;
using System.Linq;
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

        [Theory, InlineData("", "!"), InlineData("", "22"), InlineData("!", "!"), InlineData("!", "22"), InlineData("22", "!"), InlineData("22", "22")]
        public void MatchesEmbeddedSequencePatterns(string prefix, string suffix)
        {
            const string pattern = "jihg";

            var password = prefix + pattern + suffix;
            var i = prefix.Length;
            var j = i + pattern.Length - 1;

            var res = _matcher.MatchPassword(password).OfType<SequenceMatch>().ToList();

            res.Should().HaveCount(1);

            res[0].Pattern.Should().Be("sequence");
            res[0].Token.Should().Be(pattern);
            res[0].i.Should().Be(i);
            res[0].j.Should().Be(j);
            res[0].Ascending.Should().Be(false);
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

        [Theory,
         InlineData("ABC", "upper", true), InlineData("CBA", "upper", false), InlineData("PQR", "upper", true), InlineData("RQP", "upper", false), InlineData("XYZ", "upper", true), InlineData("ZYX", "upper", false),
         InlineData("abcd", "lower", true), InlineData("dcba", "lower", false), InlineData("jihg", "lower", false), InlineData("wxyz", "lower", true), InlineData("zxvt", "lower", false),
         InlineData("0369", "digits", true), InlineData("97531", "digits", false)
        ]
        public void MatchesSpecificSequence(string password, string name, bool ascending)
        {
            var res = _matcher.MatchPassword(password).OfType<SequenceMatch>().ToList();

            res.Should().HaveCount(1);

            res[0].Pattern.Should().Be("sequence");
            res[0].Token.Should().Be(password);
            res[0].i.Should().Be(0);
            res[0].j.Should().Be(password.Length - 1);
            res[0].Ascending.Should().Be(ascending);
            res[0].SequenceName = name;
        }
    }
}