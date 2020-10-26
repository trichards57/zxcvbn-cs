using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class DictionaryMatcherTests
    {
        private readonly DictionaryMatcher _matcher1 = new DictionaryMatcher("d1", "test_dictionary_1.txt");
        private readonly DictionaryMatcher _matcher2 = new DictionaryMatcher("d2", "test_dictionary_2.txt");
        private readonly DictionaryMatcher _matcherTv = new DictionaryMatcher("us_tv_and_film", "us_tv_and_film.lst");

        [Theory, InlineData("q", "%"), InlineData("q", "qq"), InlineData("%%", "%"), InlineData("%%", "qq")]
        public void IdentifiesWordsSurroundedByNonWords(string prefix, string suffix)
        {
            var word = "asdf1234&*";
            var password = $"{prefix}{word}{suffix}";
            var result = RunMatches(password);

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = "d2",
                i = prefix.Length,
                j = prefix.Length+word.Length-1,
                MatchedWord = word,
                Rank = 5,
                Reversed = false,
                L33t = false,
                Token = word
            } };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IgnoresUppercasing()
        {
            var result = RunMatches("BoaRdZ");

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 0,
                j = 4,
                MatchedWord = "board",
                Rank = 3,
                Reversed = false,
                L33t = false,
                Token = "BoaRd"
            } ,
                new DictionaryMatch{
                Pattern="dictionary",
                DictionaryName = "d2",
                i = 5,
                j = 5,
                MatchedWord = "z",
                Rank = 1,
                Reversed = false,
                L33t = false,
                Token = "Z"
            }};
            result.Should().BeEquivalentTo(expected);
        }

        [Theory, InlineData("mother", 2, "d1"), InlineData("board", 3, "d1"), InlineData("abcd", 4, "d1"), InlineData("cdef", 5, "d1"),
         InlineData("z", 1, "d2"), InlineData("8", 2, "d2"), InlineData("99", 3, "d2"), InlineData("$", 4, "d2"), InlineData("asdf1234&*", 5, "d2")]
        public void MatchesAgainstAllWordsInProvidedDictionaries(string word, int rank, string dictionary)
        {
            var result = RunMatches(word);

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = dictionary,
                i = 0,
                j = word.Length-1,
                MatchedWord = word,
                Rank = rank,
                Reversed = false,
                L33t = false,
                Token = word
            } };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesMultipleOverlappingWords()
        {
            var result = RunMatches("abcdef");

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 0,
                j = 3,
                MatchedWord = "abcd",
                Rank = 4,
                Reversed = false,
                L33t = false,
                Token = "abcd"
            } ,
                new DictionaryMatch{
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 2,
                j = 5,
                MatchedWord = "cdef",
                Rank = 5,
                Reversed = false,
                L33t = false,
                Token = "cdef"
            }};
            result.Should().BeEquivalentTo(expected);
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
        public void MatchesWordsThatContainOtherWords()
        {
            var result = RunMatches("motherboard");

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 0,
                j = 5,
                MatchedWord = "mother",
                Rank = 2,
                Reversed = false,
                L33t = false,
                Token = "mother"
            } ,
                new DictionaryMatch{
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 0,
                j = 10,
                MatchedWord = "motherboard",
                Rank = 1,
                Reversed = false,
                L33t = false,
                Token = "motherboard"
            },new DictionaryMatch{
                Pattern="dictionary",
                DictionaryName = "d1",
                i = 6,
                j = 10,
                MatchedWord = "board",
                Rank = 3,
                Reversed = false,
                L33t = false,
                Token = "board"
            }};

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void UsesTheDefaultDictionaries()
        {
            var result = _matcherTv.MatchPassword("wow").OfType<DictionaryMatch>().ToList();

            var expected = new[] { new DictionaryMatch
            {
                Pattern="dictionary",
                DictionaryName = "us_tv_and_film",
                i = 0,
                j = 2,
                MatchedWord = "wow",
                Rank = 322,
                Reversed = false,
                L33t = false,
                Token = "wow"
            } };
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void UsesTheUserInputDictionary()
        {
            var matcher = new DictionaryMatcher("user_inputs", new[] { "foo", "bar" });

            var result = matcher.MatchPassword("foobar").OfType<DictionaryMatch>().ToList();

            var expected = new[] {
                new DictionaryMatch
                {
                    Pattern="dictionary",
                    DictionaryName = "user_inputs",
                    i = 0,
                    j = 2,
                    MatchedWord = "foo",
                    Rank = 1,
                    Reversed = false,
                    L33t = false,
                    Token = "foo"
                },
                new DictionaryMatch
                {
                    Pattern="dictionary",
                    DictionaryName = "user_inputs",
                    i = 3,
                    j = 5,
                    MatchedWord = "bar",
                    Rank = 2,
                    Reversed = false,
                    L33t = false,
                    Token = "bar"
                }
            };

            result.Should().BeEquivalentTo(expected);
        }

        private List<DictionaryMatch> RunMatches(string word)
        {
            var result = _matcher1.MatchPassword(word).Concat(_matcher2.MatchPassword(word));

            return result.OfType<DictionaryMatch>().ToList();
        }
    }
}
