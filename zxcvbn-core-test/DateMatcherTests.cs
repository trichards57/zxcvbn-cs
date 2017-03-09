using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;

namespace Zxcvbn.Tests
{
    public class DateMatcherTests
    {
        [Theory, InlineData("m/d/y"), InlineData("d/m/y"),
            InlineData("y/m/d"), InlineData("y/d/m")]
        public void DateMatcherMatchesDateInDifferentOrders(string order)
        {
            var day = 21;
            var month = 8;
            var year = 1935;

            var password = order.Replace("d", day.ToString())
                .Replace("m", month.ToString()).Replace("y", year.ToString());

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password);

            matches.Count().Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().BeGreaterOrEqualTo(1);
            matches.OfType<DateMatch>().Count(m => m.Year == year).Should().Be(1);

            var match = matches.OfType<DateMatch>().Single(m => m.Year == year);
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("/");
            match.Year.Should().Be(year);
            match.Month.Should().Be(month);
            match.Day.Should().Be(day);
        }

        [Fact]
        public void DateMatcherMatchesDateUsingNoSeperator()
        {
            var password = $"1321921";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password);

            matches.Should().HaveCount(1);

            // Only the whole date is matched is matched (makes no attempt
            // to break up the date).
            var basicMatch = matches.Single();
            basicMatch.i.Should().Be(0);
            basicMatch.j.Should().Be(password.Length - 1);
            basicMatch.Pattern.Should().Be("date");
            basicMatch.Token.Should().Be("1321921");
        }

        [Theory, InlineData(" "), InlineData("-"), InlineData("/"),
                    InlineData("\\"), InlineData("_"), InlineData(".")]
        public void DateMatcherMatchesDateUsingSeperator(string seperator)
        {
            var password = $"13{seperator}2{seperator}1921";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password);

            matches.Count().Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().BeGreaterOrEqualTo(1);
            matches.OfType<DateMatch>().Count(m => m.Year == 1921).Should().Be(1);

            var match = matches.OfType<DateMatch>().Single(m => m.Year == 1921);
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be(seperator);
            match.Year.Should().Be(1921);
            match.Month.Should().Be(2);
            match.Day.Should().Be(13);
        }

        [Theory, InlineData("a", ""), InlineData("ab", ""), InlineData("", "!"),
            InlineData("a", "!"), InlineData("ab", "!")]
        public void DateMatcherMatchesDateWithPrefixAndOrSuffix(string prefix, string suffix)
        {
            var password = $"{prefix}1/1/91{suffix}";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password);

            matches.Count().Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().BeGreaterOrEqualTo(1);
            matches.OfType<DateMatch>().Count(m => m.Year == 91).Should().Be(1);

            var match = matches.OfType<DateMatch>().Single(m => m.Year == 91);
            match.Pattern.Should().Be("date");
            match.Token.Should().Be("1/1/91");
            match.i.Should().Be(prefix.Length);
            match.j.Should().Be(password.Length - 1 - suffix.Length);
            match.Separator.Should().Be("/");
            match.Year.Should().Be(91);
            match.Month.Should().Be(1);
            match.Day.Should().Be(1);
        }
    }
}