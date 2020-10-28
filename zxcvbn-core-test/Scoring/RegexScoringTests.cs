using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Scoring
{
    public class RegexScoringTests
    {
        [Fact]
        public void CalculatesWithReferenceYear()
        {
            var match = new RegexMatch
            {
                Token = "1972",
                RegexName = "recent_year",
                i = 1,
                j = 2,
            };

            var result = PasswordScoring.EstimateGuesses(match, "1972");
            result.Should().Be(DateMatcher.ReferenceYear - 1972);
        }

        [Fact]
        public void IsDelegatedToByEstimateGuesses()
        {
            var match = new RegexMatch
            {
                Token = "1972",
                RegexName = "recent_year",
                i = 1,
                j = 2,
            };

            var result = PasswordScoring.EstimateGuesses(match, "1972");
            result.Should().Be(DateMatcher.ReferenceYear - 1972);
        }
    }
}
