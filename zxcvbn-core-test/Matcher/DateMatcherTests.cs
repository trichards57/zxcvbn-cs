using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class DateMatcherTests
    {
        [Fact]
        public void DateMatcherMatchesDatesPaddedByNonAmbiguousDigits()
        {
            const string password = "912/20/919";

            var expected = new[] { new DateMatch
            {
                i = 1,
                j = 8,
                Token = "12/20/91",
                Day= 20,
                Month=12,
                Year = 1991,
                Separator = "/",
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }

        [Theory(DisplayName = "DateMatcher.DifferentSeperator"), InlineData(""), InlineData(" "), InlineData("-"), InlineData("/"),
                            InlineData("\\"), InlineData("_"), InlineData(".")]
        public void DateMatcherMatchesDatesWithVariedSeperators(string seperator)
        {
            var password = $"13{seperator}2{seperator}1921";

            var expected = new[] { new DateMatch
            {
                i = 0,
                j = password.Length-1,
                Token = password,
                Day= 13,
                Month=2,
                Year = 1921,
                Separator = seperator,
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }

        [Theory(DisplayName = "DateMatcher.DifferentOrders"), InlineData("mdy"), InlineData("dmy"),
                                            InlineData("ymd"), InlineData("ydm")]
        public void DateMatcherMatchesDateWithVariedElementOrder(string order)
        {
            const int day = 22;
            const int month = 12;
            const int year = 1935;

            var password = order.Replace("d", day.ToString())
                .Replace("m", month.ToString()).Replace("y", year.ToString());

            var expected = new[] { new DateMatch
            {
                i = 0,
                j = password.Length-1,
                Token = password,
                Day= day,
                Month=month,
                Year = year,
                Separator = "",
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }

        [Theory(DisplayName = "DateMatcher.PrefixSuffix"), InlineData("a", ""), InlineData("ab", ""), InlineData("", "!"),
                    InlineData("a", "!"), InlineData("ab", "!")]
        public void DateMatcherMatchesEmbeddedDates(string prefix, string suffix)
        {
            var pattern = "1/1/91";
            var password = $"{prefix}{pattern}{suffix}";

            var expected = new[] { new DateMatch
            {
                i = prefix.Length,
                j = prefix.Length + pattern.Length-1,
                Token = "1/1/91",
                Day= 1,
                Month=1,
                Year = 1991,
                Separator = "/",
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DateMatcherMatchesOverlappingDates()
        {
            const string password = "12/20/1991.12.20";

            var expected = new[] { new DateMatch
            {
                i =0,
                j = 9,
                Token = "12/20/1991",
                Day= 20,
                Month=12,
                Year = 1991,
                Separator = "/"
            },
            new DateMatch
            {
                  i =6,
                j = 15,
                Token = "1991.12.20",
                Day= 20,
                Month=12,
                Year = 1991,
                Separator = "."
            }};

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DateMatcherMatchesTheDateWithYearClosestToReferenceYear()
        {
            const string password = "111504";

            var expected = new[] { new DateMatch
            {
                i = 0,
                j = password.Length - 1,
                Token = password,
                Day = 15,
                Month = 11,
                Year = 2004,
                Separator = ""
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();
            matches.Should().BeEquivalentTo(expected);
        }

        [Theory, InlineData(1, 1, 1999), InlineData(11, 8, 2000), InlineData(9, 12, 2005), InlineData(22, 11, 1551)]
        public void DateMatcherMatchesVariousDates(int day, int month, int year)
        {
            var password = $"{year}{month}{day}";

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().HaveCountGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().Be(1);

            var match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(password);
            match.i.Should().Be(0);
            match.j.Should().Be(password.Length - 1);
            match.Separator.Should().Be("");
            match.Year.Should().Be(year);

            var dottedPassword = $"{year}.{month}.{day}";

            matches = matcher.MatchPassword(dottedPassword).ToList();

            matches.Should().HaveCountGreaterOrEqualTo(1);
            matches.Count(m => m is DateMatch).Should().Be(1);

            match = matches.OfType<DateMatch>().Single();
            match.Pattern.Should().Be("date");
            match.Token.Should().Be(dottedPassword);
            match.i.Should().Be(0);
            match.j.Should().Be(dottedPassword.Length - 1);
            match.Separator.Should().Be(".");
            match.Year.Should().Be(year);
        }

        [Fact]
        public void DateMatchesMatchesZeroPaddedDates()
        {
            const string password = "02/02/02";

            var expected = new[] { new DateMatch
            {
                i = 0,
                j = password.Length - 1,
                Token = password,
                Day = 2,
                Month = 2,
                Year = 2002,
                Separator = "/"
            } };

            var matcher = new DateMatcher();

            var matches = matcher.MatchPassword(password).ToList();

            matches.Should().BeEquivalentTo(expected);
        }
    }
}
