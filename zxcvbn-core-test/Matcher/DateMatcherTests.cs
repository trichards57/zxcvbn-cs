using System.Linq;
using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class DateMatcherTests
    {
        [Theory, InlineData("a", ""), InlineData("ab", ""), InlineData("", "!"),
            InlineData("a", "!"), InlineData("ab", "!")]
        public void DateMatcherMatchesDateWithPrefixAndOrSuffix(string prefix, string suffix)
        {
            var password = $"{prefix}1/1/91{suffix}";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Count.Should().Be(1);
            matches.OfType<DateMatch>().Should().ContainSingle();

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
            matches.OfType<DateMatch>().Should().HaveCount(2);
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

        [Theory, InlineData("mdy"), InlineData("dmy"), InlineData("ymd"), InlineData("ydm")]
        public void MatchesDateInDifferentOrders(string order)
        {
            const int day = 8;
            const int month = 8;
            const int year = 88;

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
            match.Year.Should().Be(1988);
            match.Month.Should().Be(8);
            match.Day.Should().Be(8);
        }

        [Theory, InlineData(""), InlineData(" "), InlineData("-"), InlineData("/"), InlineData("\\"), InlineData("_"), InlineData(".")]
        public void MatchesDateUsingSeperator(string seperator)
        {
            var password = $"13{seperator}2{seperator}1921";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.OfType<DateMatch>().Should().ContainSingle();

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

        [Theory, InlineData(1, 1, 1999), InlineData(11, 8, 2000), InlineData(9, 12, 2005), InlineData(22, 11, 1551)]
        public void MatchesPassword(int day, int month, int year)
        {
            var password = $"{day}{month}{year}";

            var matcher = new DateMatcher();
            var matches = matcher.MatchPassword(password).ToList();

            matches.OfType<DateMatch>().Should().ContainSingle();

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("");
            match.Year.Should().Be(year);

            password = $"{day}.{month}.{year}";

            matches = matcher.MatchPassword(password).ToList();

            matches.OfType<DateMatch>().Should().ContainSingle();

            match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be(".");
            match.Year.Should().Be(year);
        }

        [Fact]
        public void MatchesTheDateWithYearClosestToReferenceYearWhenAmbiguous()
        {
            var password = "111504";
            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.OfType<DateMatch>().Should().ContainSingle();

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("");
            match.Year.Should().Be(2004);
            match.Month.Should().Be(11);
            match.Day.Should().Be(15);
        }

        [Fact]
        public void MatchesZeroPaddedDates()
        {
            var password = $"02/02/02";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.OfType<DateMatch>().Should().ContainSingle();

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("/");
            match.Year.Should().Be(2002);
            match.Month.Should().Be(2);
            match.Day.Should().Be(2);
        }
    }
}