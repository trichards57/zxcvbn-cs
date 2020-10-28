using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Scoring
{
    public class ScoringTests
    {
        [Fact]
        public void ReturnsCachedGuessesIfAvailable()
        {
            var match = new DateMatch
            {
                Guesses = 1,
                Token = "1977",
                Year = 1977,
                Month = 8,
                Day = 14,
                Separator = "/",
                i = 1,
                j = 2,
            };

            var actual = PasswordScoring.EstimateGuesses(match, string.Empty);
            actual.Should().Be(1);
        }
    }
}
