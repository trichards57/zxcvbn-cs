using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class RegexMatcherTests
    {
        [Theory, InlineData("1922", "recent_year"), InlineData("2017", "recent_year")]
        public void MatchesPattern(string pattern, string name)
        {
            var re = new RegexMatcher(pattern, name);

            var result = re.MatchPassword(pattern).OfType<RegexMatch>().ToList();

            var expected = new[] { new RegexMatch
            {
                Pattern="regex",
                i = 0,
                j = pattern.Length-1,
                RegexName = name,
                Token = pattern
            } };
            result.Should().BeEquivalentTo(expected);
        }
    }
}
