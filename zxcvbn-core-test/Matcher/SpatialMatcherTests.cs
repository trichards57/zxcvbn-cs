using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class SpatialMatcherTests
    {
        private readonly SpatialMatcher matcher = new SpatialMatcher();

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("qw")]
        [InlineData("*/")]
        public void DoesNotmatch1And2CharacterSpatialPatterns(string password)
        {
            var matches = matcher.MatchPassword(password);

            matches.Should().BeEmpty();
        }

        [Theory]
        [InlineData("12345", "qwerty", 1, 0)]
        [InlineData("@WSX", "qwerty", 1, 4)]
        [InlineData("6tfGHJ", "qwerty", 2, 3)]
        [InlineData("hGFd", "qwerty", 1, 2)]
        [InlineData("/;p09876yhn", "qwerty", 3, 0)]
        [InlineData("Xdr%", "qwerty", 1, 2)]
        [InlineData("159-", "keypad", 1, 0)]
        [InlineData("*84", "keypad", 1, 0)]
        [InlineData("/8520", "keypad", 1, 0)]
        [InlineData("369", "keypad", 1, 0)]
        [InlineData("/963.", "mac_keypad", 1, 0)]
        [InlineData("*-632.0214", "mac_keypad", 9, 0)]
        [InlineData("aoEP%yIxkjq:", "dvorak", 4, 5)]
        [InlineData(";qoaOQ:Aoq;a", "dvorak", 11, 4)]
        public void MatchesGeneralPattern(string pattern, string keyboard, int turns, int shifts)
        {
            var result = matcher.MatchPassword(pattern).OfType<SpatialMatch>().Where(m => m.Graph == keyboard).ToList();
            var expected = new[]
            {
                new SpatialMatch
                {
                    Graph = keyboard,
                    Turns = turns,
                    ShiftedCount = shifts,
                    i = 0,
                    j = pattern.Length - 1,
                    Token = pattern,
                },
            };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesSpatialPatternSurroundedByNonSpatialPatterns()
        {
            var matcher = new SpatialMatcher();
            var pattern = "6tfGHJ";
            var password = $"rz!{pattern}%z";
            var result = matcher.MatchPassword(password).OfType<SpatialMatch>().Where(m => m.Graph == "qwerty").ToList();

            var expected = new[]
            {
                new SpatialMatch
                {
                    Graph = "qwerty",
                    Turns = 2,
                    ShiftedCount = 3,
                    i = 3,
                    j = 3 + pattern.Length - 1,
                    Token = pattern,
                },
            };
            result.Should().BeEquivalentTo(expected);
        }
    }
}
