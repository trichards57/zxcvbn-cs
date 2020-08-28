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
            result.Should().ContainSingle();

            result[0].Token.Should().Be(pattern);
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(pattern.Length - 1);
            result[0].Pattern.Should().Be("regex");
            result[0].RegexName.Should().Be(name);
        }
    }
}