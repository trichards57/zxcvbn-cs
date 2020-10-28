using FluentAssertions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class L33tMatcherTests
    {
        private static readonly DictionaryMatcher TestDictionary1 = new DictionaryMatcher("words", new[] { "aac", string.Empty, "password", "paassword", "asdf0" });

        private static readonly DictionaryMatcher TestDictionary2 = new DictionaryMatcher("words2", new[] { "cgo" });

        private static readonly ReadOnlyDictionary<char, char[]> TestL33tTable = new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
        {
            ['a'] = new[] { '4', '@' },
            ['c'] = new[] { '(', '{', '[', '<' },
            ['g'] = new[] { '6', '9' },
            ['o'] = new[] { '0' },
        });

        [Fact]
        public void DoesNotMatchEmptyString()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var result = matcher.MatchPassword(string.Empty);
            result.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotMatchNonL33tWords()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var result = matcher.MatchPassword("password");
            result.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotMatchSingleCharacterL33tedWords()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var result = matcher.MatchPassword("4 1 @");
            result.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotMatchWhenMultipleSubstitutionsAreNeededForTheSameLetter()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var result = matcher.MatchPassword("p4@ssword");
            result.Should().BeEmpty();
        }

        [Fact]
        public void DoesNotMatchWithSubsetsOfPossibleL33tCombinations()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var result = matcher.MatchPassword("4sdf0");
            result.Should().BeEmpty();
        }

        [Fact]
        public void EnumerateL33tSubsGetsTheSetOfSubstitutionsForAPassword()
        {
            L33tMatcher.L33tTable = TestL33tTable;

            var actual = L33tMatcher.EnumerateSubtitutions(new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>()));
            actual.Single().Should().BeEmpty();

            actual = L33tMatcher.EnumerateSubtitutions(new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
            {
                { 'a', new[] { '@' } },
            }));
            var expected = new List<Dictionary<char, char>>()
            {
                new Dictionary<char, char>()
                {
                    { '@', 'a' },
                },
            };
            actual.Should().BeEquivalentTo(expected);

            actual = L33tMatcher.EnumerateSubtitutions(new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
            {
                { 'a', new[] { '@', '4' } },
            }));
            expected = new List<Dictionary<char, char>>()
            {
                new Dictionary<char, char>()
                {
                    { '@', 'a' },
                },
                new Dictionary<char, char>()
                {
                    { '4', 'a' },
                },
            };
            actual.Should().BeEquivalentTo(expected);

            actual = L33tMatcher.EnumerateSubtitutions(new ReadOnlyDictionary<char, char[]>(new Dictionary<char, char[]>
            {
                { 'a', new[] { '@', '4' } },
                { 'c', new[] { '(' } },
            }));
            expected = new List<Dictionary<char, char>>()
            {
                new Dictionary<char, char>()
                {
                    { '@', 'a' },
                    { '(', 'c' },
                },
                new Dictionary<char, char>()
                {
                    { '4', 'a' },
                    { '(', 'c' },
                },
            };
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesCommonL33tSubstitutions()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var expected = new List<DictionaryMatch>
            {
                new DictionaryMatch()
                {
                    DictionaryName = "words",
                    i = 0,
                    j = 7,
                    MatchedWord = "password",
                    Rank = 3,
                    Reversed = false,
                    L33t = true,
                    Token = "p4ssword",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '4', 'a' },
                    },
                },
            };
            var result = matcher.MatchPassword("p4ssword");
            result.Should().BeEquivalentTo(expected);

            expected = new List<DictionaryMatch>
            {
                new DictionaryMatch()
                {
                    DictionaryName = "words",
                    i = 0,
                    j = 7,
                    MatchedWord = "password",
                    Rank = 3,
                    Reversed = false,
                    L33t = true,
                    Token = "p@ssw0rd",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '@', 'a' }, { '0', 'o' },
                    },
                },
            };
            result = matcher.MatchPassword("p@ssw0rd");
            result.Should().BeEquivalentTo(expected);

            expected = new List<DictionaryMatch>
            {
                new DictionaryMatch()
                {
                    DictionaryName = "words2",
                    i = 5,
                    j = 7,
                    MatchedWord = "cgo",
                    Rank = 1,
                    Reversed = false,
                    L33t = true,
                    Token = "{G0",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '{', 'c' }, { '0', 'o' },
                    },
                },
            };
            result = matcher.MatchPassword("aSdfO{G0asDfO");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesOveralppingL33tPatterns()
        {
            L33tMatcher.L33tTable = TestL33tTable;
            var matcher = new L33tMatcher(new List<IMatcher> { TestDictionary1, TestDictionary2 });

            var expected = new List<DictionaryMatch>
            {
                new DictionaryMatch()
                {
                    DictionaryName = "words",
                    i = 0,
                    j = 2,
                    MatchedWord = "aac",
                    Rank = 1,
                    Reversed = false,
                    L33t = true,
                    Token = "@a(",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '@', 'a' },
                        { '(', 'c' },
                    },
                },
                new DictionaryMatch()
                {
                    DictionaryName = "words2",
                    i = 2,
                    j = 4,
                    MatchedWord = "cgo",
                    Rank = 1,
                    Reversed = false,
                    L33t = true,
                    Token = "(go",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '(', 'c' },
                    },
                },
                new DictionaryMatch()
                {
                    DictionaryName = "words2",
                    i = 5,
                    j = 7,
                    MatchedWord = "cgo",
                    Rank = 1,
                    Reversed = false,
                    L33t = true,
                    Token = "{G0",
                    L33tSubs = new Dictionary<char, char>
                    {
                        { '{', 'c' }, { '0', 'o' },
                    },
                },
            };
            var result = matcher.MatchPassword("@a(go{G0");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void RelevantL33tTableReducesSubstitutions()
        {
            L33tMatcher.L33tTable = TestL33tTable;

            var actual = L33tMatcher.RelevantL33tSubtable(string.Empty);
            actual.Should().BeEmpty();

            actual = L33tMatcher.RelevantL33tSubtable("abcdefgo123578!#$&*)]}>");
            actual.Should().BeEmpty();

            actual = L33tMatcher.RelevantL33tSubtable("a");
            actual.Should().BeEmpty();

            var expected = new Dictionary<char, char[]>
            {
                { 'a', new[] { '4' } },
            };
            actual = L33tMatcher.RelevantL33tSubtable("4");
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
