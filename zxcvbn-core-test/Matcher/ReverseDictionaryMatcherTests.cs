using FluentAssertions;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class ReverseDictionaryMatcherTests
    {
        private readonly ReverseDictionaryMatcher _matcher = new ReverseDictionaryMatcher("d3", "test_dictionary_3.txt");

        [Fact]
        public void MatchesAganstReversedWords()
        {
            var result = _matcher.MatchPassword("0123456789").OfType<DictionaryMatch>().ToList();

            result.Should().HaveCount(2);

            result[0].Token.Should().Be("123");
            result[0].MatchedWord.Should().Be("321");
            result[0].Rank.Should().Be(2);
            result[0].i.Should().Be(1);
            result[0].j.Should().Be(3);
            result[0].Reversed.Should().BeTrue();
            result[0].DictionaryName.Should().Be("d3");

            result[1].Token.Should().Be("456");
            result[1].MatchedWord.Should().Be("654");
            result[1].Rank.Should().Be(4);
            result[1].i.Should().Be(4);
            result[1].j.Should().Be(6);
            result[1].Reversed.Should().BeTrue();
            result[1].DictionaryName.Should().Be("d3");
        }
    }
}