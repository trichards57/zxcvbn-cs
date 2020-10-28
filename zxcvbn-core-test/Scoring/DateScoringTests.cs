using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn.Tests.Scoring
{
    public class DateScoringTests
    {
        [Fact]
        public void AssumesMinYearSpaceForRecentYears()
        {
            var match = new DateMatch
            {
                Token = "2010",
                Separator = "",
                Year = 2010,
                Month = 1,
                Day = 1,
                i = 1,
                j = 2,
            };

            var actual = DateGuessesCalculator.CalculateGuesses(match);
            var expected = 365 * DateGuessesCalculator.MinimumYearSpace;

            actual.Should().Be(expected);
        }

        [Fact]
        public void CalculatesBasedOnReferenceYear()
        {
            var match = new DateMatch
            {
                Token = "1923",
                Separator = "",
                Year = 1923,
                Month = 1,
                Day = 1,
                i = 1,
                j = 2,
            };

            var actual = DateGuessesCalculator.CalculateGuesses(match);
            var expected = 365 * (DateMatcher.ReferenceYear - match.Year);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsDelegatedToBeEstimateGuesses()
        {
            var match = new DateMatch
            {
                Token = "1923",
                Separator = "",
                Year = 1923,
                Month = 1,
                Day = 1,
                i = 1,
                j = 2,
            };

            var actual = PasswordScoring.EstimateGuesses(match, "1923");
            var expected = 365 * (DateMatcher.ReferenceYear - match.Year);

            actual.Should().Be(expected);
        }

        [Fact]
        public void MultipliesByFourForSeparators()
        {
            var match = new DateMatch
            {
                Token = "1923",
                Separator = "/",
                Year = 1923,
                Month = 1,
                Day = 1,
                i = 1,
                j = 2,
            };

            var actual = DateGuessesCalculator.CalculateGuesses(match);
            var expected = 365 * (DateMatcher.ReferenceYear - match.Year) * 4;

            actual.Should().Be(expected);
        }
    }
}
