using System.Linq;
using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class SpatialMatcherTests
    {
        private readonly SpatialMatcher _matcher = new SpatialMatcher();

        [Theory, InlineData(""), InlineData("/"), InlineData("qw"), InlineData("*/")]
        public void DoesNotmatch1And2CharacterPatterns(string password)
        {
            var matches = _matcher.MatchPassword(password);

            matches.Should().BeEmpty();
        }

        [Theory, InlineData("12345", "qwerty", 1, 0), InlineData("@WSX", "qwerty", 1, 4), InlineData("6tfGHJ", "qwerty", 2, 3), InlineData("hGFd", "qwerty", 1, 2),
         InlineData("/;p09876yhn", "qwerty", 3, 0), InlineData("Xdr%", "qwerty", 1, 2), InlineData("159-", "keypad", 1, 0),
         InlineData("*84", "keypad", 1, 0), InlineData("/8520", "keypad", 1, 0), InlineData("369", "keypad", 1, 0), InlineData("/963.", "mac_keypad", 1, 0),
         InlineData("*-632.0214", "mac_keypad", 9, 0), InlineData("aoEP%yIxkjq:", "dvorak", 4, 5), InlineData(";qoaOQ:Aoq;a", "dvorak", 11, 4)]
        public void MatchesKeyboardPattern(string pattern, string keyboard, int turns, int shifts)
        {
            var matches = _matcher.MatchPassword(pattern).OfType<SpatialMatch>().ToList();

            matches.Should().NotBeEmpty();
            matches.Should().ContainSingle(s => s.Graph == keyboard);

            var match = matches.Single(m => m.Graph == keyboard);

            match.Turns.Should().Be(turns);
            match.ShiftedCount.Should().Be(shifts);
            match.i.Should().Be(0);
            match.j.Should().Be(pattern.Length - 1);
            match.Pattern.Should().Be("spatial");
        }
    }
}