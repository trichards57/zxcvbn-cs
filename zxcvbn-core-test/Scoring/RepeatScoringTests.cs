using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn.Tests.Scoring
{
    public class RepeatScoringTests
    {
        [Theory]
        [InlineData("aa", "a", 2)]
        [InlineData("999", "9", 3)]
        [InlineData("$$$$", "$", 4)]
        [InlineData("abab", "ab", 2)]
        [InlineData("batterystaplebatterystaplebatterystaple", "batterystaple", 3)]
        public void CalculatesTheRightNumberOfGuesses(string token, string baseToken, int expectedRepeats)
        {
            var baseGuesses = PasswordScoring.MostGuessableMatchSequence(baseToken, Core.GetAllMatches(baseToken)).Guesses;

            var match = new RepeatMatch
            {
                Token = token,
                BaseToken = baseToken,
                BaseGuesses = baseGuesses,
                RepeatCount = expectedRepeats,
                BaseMatchItems = new List<Match>(),
                i = 1,
                j = 2,
            };

            var expected = baseGuesses * expectedRepeats;

            var actual = RepeatGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void IsDelegatedToByEstimateGuesses()
        {
            var match = new RepeatMatch
            {
                Token = "aa",
                BaseToken = "a",
                BaseGuesses = PasswordScoring.MostGuessableMatchSequence("a", Core.GetAllMatches("a")).Guesses,
                BaseMatchItems = new List<Match>(),
                RepeatCount = 2,
                i = 1,
                j = 2,
            };

            var expected = RepeatGuessesCalculator.CalculateGuesses(match);
            var actual = PasswordScoring.EstimateGuesses(match, "aa");

            actual.Should().Be(expected);
        }
    }
}
