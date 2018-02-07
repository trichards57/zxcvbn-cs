//using System.Linq;
//using FluentAssertions;
//using Xunit;
//using Zxcvbn.Matcher;

//namespace Zxcvbn.Tests.Matcher
//{
//    public class SequenceMatcherTests
//    {
//        [Fact]
//        public void MultipleSequence()
//        {
//            var seq = new SequenceMatcher();

//            var res = seq.MatchPassword("asdfabcdhujzyxwhgjj").ToList();
//            res.Count.Should().Be(2);

//            res[0].i.Should().Be(4);
//            res[0].j.Should().Be(7);
//            res[0].Token.Should().Be("abcd");

//            res[1].i.Should().Be(11);
//            res[1].j.Should().Be(14);
//            res[1].Token.Should().Be("zyxw");
//        }

//        [Fact]
//        public void NoSequence()
//        {
//            var seq = new SequenceMatcher();

//            var res = seq.MatchPassword("dfsjkhfjksdh").ToList();
//            res.Should().BeEmpty();
//        }

//        [Fact]
//        public void SingleSequence()
//        {
//            var seq = new SequenceMatcher();

//            var res = seq.MatchPassword("abcd").ToList();
//            res.Count.Should().Be(1);
//            res[0].i.Should().Be(0);
//            res[0].j.Should().Be(3);
//            res[0].Token.Should().Be("abcd");
//        }
//    }
//}