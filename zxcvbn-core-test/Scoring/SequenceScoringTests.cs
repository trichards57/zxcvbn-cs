using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn.Tests.Scoring
{
    public class SequenceScoringTests
    {
        [Theory]
        [InlineData("ab", true, 8)]
        [InlineData("XYZ", true, 78)]
        [InlineData("4567", true, 40)]
        [InlineData("7654", false, 80)]
        [InlineData("ZYX", false, 24)]
        public void CalculatesTheCorrectNumberOfGuesses(string token, bool ascending, int expected)
        {
            var match = new SequenceMatch
            {
                Token = token,
                Ascending = ascending,
                i = 1,
                j = 2,
                SequenceName = "abc",
                SequenceSpace = 1,
            };

            var actual = SequenceGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void IsDelegatedToByEstimateGuesses()
        {
            var match = new SequenceMatch
            {
                Token = "ab",
                Ascending = true,
                i = 1,
                j = 2,
                SequenceName = "abc",
                SequenceSpace = 1,
            };

            var actual = SequenceGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(8);
        }
    }
}
