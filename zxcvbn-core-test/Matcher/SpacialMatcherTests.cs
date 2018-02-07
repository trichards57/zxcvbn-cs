//using System.Linq;
//using FluentAssertions;
//using Xunit;
//using Zxcvbn.Matcher;

//namespace Zxcvbn.Tests.Matcher
//{
//    public class SpacialMatcherTests
//    {
//        [Fact]
//        public void MultipleSequencesMultipleKeyboards()
//        {
//            var sm = new SpatialMatcher();

//            var res = sm.MatchPassword("plko14569852pyfdb").ToList();
//            res.Count.Should().Be(6);
//        }

//        [Fact]
//        public void SingleSequence()
//        {
//            var sm = new SpatialMatcher();

//            var res = sm.MatchPassword("qwert").ToList();
//            res.Count.Should().Be(1);

//            res[0].i.Should().Be(0);
//            res[0].j.Should().Be(4);
//            res[0].Token.Should().Be("qwert");
//        }
//    }
//}