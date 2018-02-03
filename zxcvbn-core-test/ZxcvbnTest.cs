using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zxcvbn;

namespace zxcvbn_test
{
    public class ZxcvbnTest
    {
        //[Fact]
        public void DigitsRegexMatcher()
        {
            var re = new Zxcvbn.Matcher.RegexMatcher("\\d{3,}", 10);

            var res = re.MatchPassword("abc123def");
            Assert.Equal(1, res.Count());
            var m1 = res.First();
            Assert.Equal(3, m1.i);
            Assert.Equal(5, m1.j);
            Assert.Equal("123", m1.Token);

            res = re.MatchPassword("123456789a12345b1234567");
            Assert.Equal(3, res.Count());
            var m3 = res.ElementAt(2);
            Assert.Equal("1234567", m3.Token);

            res = re.MatchPassword("12");
            Assert.Equal(0, res.Count());

            res = re.MatchPassword("dfsdfdfhgjkdfngjl");
            Assert.Equal(0, res.Count());
        }

        //[Fact]
        public void RepeatMatcher()
        {
            var repeat = new Zxcvbn.Matcher.RepeatMatcher();

            var res = repeat.MatchPassword("aaasdffff");
            Assert.Equal(2, res.Count());

            var m1 = res.ElementAt(0);
            Assert.Equal(0, m1.i);
            Assert.Equal(2, m1.j);
            Assert.Equal("aaa", m1.Token);

            var m2 = res.ElementAt(1);
            Assert.Equal(5, m2.i);
            Assert.Equal(8, m2.j);
            Assert.Equal("ffff", m2.Token);

            res = repeat.MatchPassword("asdf");
            Assert.Equal(0, res.Count());
        }

        //[Fact]
        public void SequenceMatcher()
        {
            var seq = new Zxcvbn.Matcher.SequenceMatcher();

            var res = seq.MatchPassword("abcd");
            Assert.Equal(1, res.Count());
            var m1 = res.First();
            Assert.Equal(0, m1.i);
            Assert.Equal(3, m1.j);
            Assert.Equal("abcd", m1.Token);

            res = seq.MatchPassword("asdfabcdhujzyxwhgjj");
            Assert.Equal(2, res.Count());

            m1 = res.ElementAt(0);
            Assert.Equal(4, m1.i);
            Assert.Equal(7, m1.j);
            Assert.Equal("abcd", m1.Token);

            var m2 = res.ElementAt(1);
            Assert.Equal(11, m2.i);
            Assert.Equal(14, m2.j);
            Assert.Equal("zyxw", m2.Token);

            res = seq.MatchPassword("dfsjkhfjksdh");
            Assert.Equal(0, res.Count());
        }

        //[Fact]
        public void SinglePasswordTest()
        {
            var res = Zxcvbn.Zxcvbn.MatchPassword("||ke");
        }

        //[Fact]
        public void SpatialMatcher()
        {
            var sm = new Zxcvbn.Matcher.SpatialMatcher();

            var res = sm.MatchPassword("qwert");
            Assert.Equal(1, res.Count());
            var m1 = res.First();
            Assert.Equal("qwert", m1.Token);
            Assert.Equal(0, m1.i);
            Assert.Equal(4, m1.j);

            res = sm.MatchPassword("plko14569852pyfdb");
            Assert.Equal(6, res.Count()); // Multiple matches from different keyboard types
        }

        //[Fact]
        public void TimeDisplayStrings()
        {
            // Note that the time strings should be + 1
            Assert.Equal("11 minutes", Utility.DisplayTime(60 * 10, Translation.English));
            Assert.Equal("2 days", Utility.DisplayTime(60 * 60 * 24, Translation.English));
            Assert.Equal("17 years", Utility.DisplayTime(60 * 60 * 24 * 365 * 15.4, Translation.English));
        }

        //[Fact]
        public void TimeDisplayStringsGerman()
        {
            // Note that the time strings should be + 1
            Assert.Equal("11 Minuten", Utility.DisplayTime(60 * 10, Translation.German));
            Assert.Equal("2 Tage", Utility.DisplayTime(60 * 60 * 24, Translation.German));
            Assert.Equal("17 Jahre", Utility.DisplayTime(60 * 60 * 24 * 365 * 15.4, Translation.German));
        }
    }
}