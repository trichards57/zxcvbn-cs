using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class DictionaryMatcherTests
    {
        private readonly DictionaryMatcher _matcher1 = new DictionaryMatcher("d1", "test_dictionary_1.txt");
        private readonly DictionaryMatcher _matcher2 = new DictionaryMatcher("d2", "test_dictionary_2.txt");

        [Theory, InlineData("qasdf1234&*%"), InlineData("qasdf1234&*qq"), InlineData("%%asdf1234&*%"), InlineData("%%asdf1234&*qq")]
        public void IdentifiesWordsSurroundedByNonWords(string word)
        {
            var result = RunMatches(word);

            result.Should().ContainSingle();

            result[0].Token.Should().Be("asdf1234&*");
            result[0].MatchedWord.Should().Be("asdf1234&*");
            result[0].Rank.Should().Be(5);
            result[0].DictionaryName.Should().Be("d2");
        }

        [Fact]
        public void IgnoresUppercasing()
        {
            var result = RunMatches("BoaRdZ");

            result.Should().HaveCount(2);

            result[0].Token.Should().Be("BoaRd");
            result[0].MatchedWord.Should().Be("board");
            result[0].Rank.Should().Be(3);
            result[0].DictionaryName.Should().Be("d1");
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(4);

            result[1].Token.Should().Be("Z");
            result[1].MatchedWord.Should().Be("z");
            result[1].Rank.Should().Be(1);
            result[1].DictionaryName.Should().Be("d2");
            result[1].i.Should().Be(5);
            result[1].j.Should().Be(5);
        }

        [Theory, InlineData("mother", 2, "d1"), InlineData("board", 3, "d1"), InlineData("abcd", 4, "d1"), InlineData("cdef", 5, "d1"),
         InlineData("z", 1, "d2"), InlineData("8", 2, "d2"), InlineData("99", 3, "d2"), InlineData("$", 4, "d2"), InlineData("asdf1234&*", 5, "d2")]
        public void MatchesAgainstAllWordsInProvidedDictionaries(string word, int rank, string dictionary)
        {
            var result = RunMatches(word);

            result.Should().HaveCount(1);

            result[0].Token.Should().Be(word);
            result[0].MatchedWord.Should().Be(word);
            result[0].Rank.Should().Be(rank);
            result[0].DictionaryName.Should().Be(dictionary);
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(word.Length - 1);
        }

        [Fact]
        public void MatchesMultipleWordsWhenTheyOverlap()
        {
            var result = RunMatches("abcdef");

            result.Should().HaveCount(2);

            result[0].Token.Should().Be("abcd");
            result[0].MatchedWord.Should().Be("abcd");
            result[0].Rank.Should().Be(4);
            result[0].DictionaryName.Should().Be("d1");
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(3);

            result[1].Token.Should().Be("cdef");
            result[1].MatchedWord.Should().Be("cdef");
            result[1].Rank.Should().Be(5);
            result[1].DictionaryName.Should().Be("d1");
            result[1].i.Should().Be(2);
            result[1].j.Should().Be(5);
        }

        [Fact]
        public void MatchesWithProvidedUserInputDictionary()
        {
            var matcher = new DictionaryMatcher("user", new[] { "foo", "bar" });
            var result = matcher.MatchPassword("foobar").OfType<DictionaryMatch>().Where(m => m.DictionaryName == "user").ToList();

            result.Should().HaveCount(2);

            result[0].Token.Should().Be("foo");
            result[0].MatchedWord.Should().Be("foo");
            result[0].Rank.Should().Be(1);
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(2);

            result[1].Token.Should().Be("bar");
            result[1].MatchedWord.Should().Be("bar");
            result[1].Rank.Should().Be(2);
            result[1].i.Should().Be(3);
            result[1].j.Should().Be(5);
        }

        [Fact]
        public void MatchesWordsContainedInWords()
        {
            var result = RunMatches("motherboard");

            result.Should().HaveCount(3);

            result[0].Token.Should().Be("mother");
            result[0].MatchedWord.Should().Be("mother");
            result[0].Rank.Should().Be(2);
            result[0].DictionaryName.Should().Be("d1");
            result[0].i.Should().Be(0);
            result[0].j.Should().Be(5);

            result[1].Token.Should().Be("motherboard");
            result[1].MatchedWord.Should().Be("motherboard");
            result[1].Rank.Should().Be(1);
            result[1].DictionaryName.Should().Be("d1");
            result[1].i.Should().Be(0);
            result[1].j.Should().Be(10);

            result[2].Token.Should().Be("board");
            result[2].MatchedWord.Should().Be("board");
            result[2].Rank.Should().Be(3);
            result[2].DictionaryName.Should().Be("d1");
            result[2].i.Should().Be(6);
            result[2].j.Should().Be(10);
        }

        private List<DictionaryMatch> RunMatches(string word)
        {
            var result = _matcher1.MatchPassword(word).Concat(_matcher2.MatchPassword(word));

            return result.OfType<DictionaryMatch>().ToList();
        }
    }
}