using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;

namespace Zxcvbn.Tests
{
    public class DateMatcherTests
    {
        [Theory(DisplayName = "DateMatcher.DifferentOrders"), InlineData("mdy"), InlineData("dmy"),
            InlineData("ymd"), InlineData("ydm")]
        public void DateMatcherMatchesDateInDifferentOrders(string order)
        {
            const int day = 22;
            const int month = 12;
            const int year = 1935;

            var password = order.Replace("d", day.ToString())
                .Replace("m", month.ToString()).Replace("y", year.ToString());

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Count.Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().Be(1);

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("");
            match.Year.Should().Be(year);
            match.Month.Should().Be(month);
            match.Day.Should().Be(day);
        }

        [Theory(DisplayName = "DateMatcher.DifferentSeperator"), InlineData(""), InlineData(" "), InlineData("-"), InlineData("/"),
                    InlineData("\\"), InlineData("_"), InlineData(".")]
        public void DateMatcherMatchesDateUsingSeperator(string seperator)
        {
            var password = $"13{seperator}2{seperator}1921";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Count.Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().Be(1);
            matches.OfType<DateMatch>().Count().Should().Be(1);

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be(seperator);
            match.Year.Should().Be(1921);
            match.Month.Should().Be(2);
            match.Day.Should().Be(13);
        }

        [Theory(DisplayName = "DateMatcher.PrefixSuffix"), InlineData("a", ""), InlineData("ab", ""), InlineData("", "!"),
            InlineData("a", "!"), InlineData("ab", "!")]
        public void DateMatcherMatchesDateWithPrefixAndOrSuffix(string prefix, string suffix)
        {
            var password = $"{prefix}1/1/91{suffix}";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Count.Should().Be(1);
            matches.OfType<DateMatch>().Count().Should().Be(1);

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be("1/1/91");
            match.i.Should().Be(prefix.Length);
            match.j.Should().Be(password.Length - 1 - suffix.Length);
            match.Separator.Should().Be("/");
            match.Year.Should().Be(1991);
            match.Month.Should().Be(1);
            match.Day.Should().Be(1);
        }

        [Fact]
        public void DateMatcherMatchesOverlappingDate()
        {
            const string password = "12/20/1991.12.20";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Count.Should().BeGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().BeGreaterOrEqualTo(2);
            matches.OfType<DateMatch>().Count(m => m.Year == 1991).Should().Be(2);

            var slashMatch = matches.OfType<DateMatch>().Single(m => m.Separator == "/");
            slashMatch.Pattern.Should().Be("date");
            slashMatch.Token.Should().Be("12/20/1991");
            slashMatch.i.Should().Be(0);
            slashMatch.j.Should().Be(9);
            slashMatch.Separator.Should().Be("/");
            slashMatch.Year.Should().Be(1991);
            slashMatch.Month.Should().Be(12);
            slashMatch.Day.Should().Be(20);

            var dotMatch = matches.OfType<DateMatch>().Single(m => m.Separator == ".");
            dotMatch.Pattern.Should().Be("date");
            dotMatch.Token.Should().Be("1991.12.20");
            dotMatch.i.Should().Be(6);
            dotMatch.j.Should().Be(15);
            dotMatch.Separator.Should().Be(".");
            dotMatch.Year.Should().Be(1991);
            dotMatch.Month.Should().Be(12);
            dotMatch.Day.Should().Be(20);
        }
    }
}