using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn.Tests.Scoring
{
    public class DictionaryScoringTests
    {
        [Fact]
        public void AddsDoubleGuessesForReversedWords()
        {
            var match = new DictionaryMatch
            {
                Token = "aaaaa",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = true,
            };

            var expected = 32 * 2;
            var actual = DictionaryGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void AddsExtraGuessesForCapitilization()
        {
            var match = new DictionaryMatch
            {
                Token = "AAAaaa",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
            };

            var expected = 32 * DictionaryGuessesCalculator.UppercaseVariations(match.Token);
            var actual = DictionaryGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void AddsExtraGuessesForCapitilizationAndCommonL33tSubstitutions()
        {
            var match = new DictionaryMatch
            {
                Token = "AaA@@@",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char> { { '@', 'a' } },
            };

            var expected = 32 * DictionaryGuessesCalculator.L33tVariations(match) * DictionaryGuessesCalculator.UppercaseVariations(match.Token);
            var actual = DictionaryGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void AddsExtraGuessesForCommonL33tSubstitutions()
        {
            var match = new DictionaryMatch
            {
                Token = "aaa@@@",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char> { { '@', 'a' } },
            };

            var expected = 32 * DictionaryGuessesCalculator.L33tVariations(match);
            var actual = DictionaryGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetsCorrectL33tVariantsForWord()
        {
            var expected = 1;
            var match = new DictionaryMatch
            {
                Token = string.Empty,
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>(),
            };
            var actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 1;
            match = new DictionaryMatch
            {
                Token = "a",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>(),
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 1;
            match = new DictionaryMatch
            {
                Token = "abcet",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>(),
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 2;
            match = new DictionaryMatch
            {
                Token = "4",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 2;
            match = new DictionaryMatch
            {
                Token = "4pple",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 2;
            match = new DictionaryMatch
            {
                Token = "4bcet",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 2;
            match = new DictionaryMatch
            {
                Token = "a8cet",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '8', 'b' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 2;
            match = new DictionaryMatch
            {
                Token = "abce+",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '+', 't' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 4;
            match = new DictionaryMatch
            {
                Token = "48cet",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' }, { '8', 'b' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 21;
            match = new DictionaryMatch
            {
                Token = "a4a4aa",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 21;
            match = new DictionaryMatch
            {
                Token = "4a4a44",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);

            expected = 30;
            match = new DictionaryMatch
            {
                Token = "a44att+",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' }, { '+', 't' } },
            };
            actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("", 1)]
        [InlineData("a", 1)]
        [InlineData("A", 2)]
        [InlineData("abcdef", 1)]
        [InlineData("Abcdef", 2)]
        [InlineData("abcdeF", 2)]
        [InlineData("ABCDEF", 2)]
        [InlineData("aBcdef", 6)]
        [InlineData("aBcDef", 21)]
        [InlineData("ABCDEf", 6)]
        [InlineData("aBCDEf", 21)]
        [InlineData("ABCdef", 41)]
        public void GetsCorrectUppercaseVariantsMultipler(string word, int expected)
        {
            var actual = DictionaryGuessesCalculator.UppercaseVariations(word);
            actual.Should().Be(expected);
        }

        [Fact]
        public void IgnoresCapitilizationForL33tVariations()
        {
            var expected = 21;
            var match = new DictionaryMatch
            {
                Token = "Aa44aA",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = true,
                MatchedWord = "a",
                Reversed = false,
                L33tSubs = new Dictionary<char, char>() { { '4', 'a' } },
            };
            var actual = DictionaryGuessesCalculator.L33tVariations(match);
            actual.Should().Be(expected);
        }

        [Fact]
        public void IsDelegatedToByEstimateGuesses()
        {
            var match = new DictionaryMatch
            {
                Token = "aaaaa",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
            };

            var expected = 32;
            var actual = PasswordScoring.EstimateGuesses(match, "aaaaa");
            actual.Should().Be(expected);
        }

        [Fact]
        public void MakesBaseGuessesEqualTheRank()
        {
            var match = new DictionaryMatch
            {
                Token = "aaaaa",
                Rank = 32,
                DictionaryName = "dic",
                i = 1,
                j = 2,
                L33t = false,
                MatchedWord = "a",
                Reversed = false,
            };

            var expected = 32;
            var actual = DictionaryGuessesCalculator.CalculateGuesses(match);
            actual.Should().Be(expected);
        }
    }
}
