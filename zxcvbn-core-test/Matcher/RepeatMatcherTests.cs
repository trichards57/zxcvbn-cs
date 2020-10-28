using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Matcher
{
    public class RepeatMatcherTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("#")]
        public void DoesNotMatchEmptyOrOneCharacterRepeatPatterns(string pattern)
        {
            var matcher = new RepeatMatcher();

            var res = matcher.MatchPassword(pattern);

            res.Should().BeEmpty();
        }

        [Fact]
        public void MatchesaabRepeatInaabaab()
        {
            var pattern = "aabaab";
            var matcher = new RepeatMatcher();

            var expected = new[]
            {
                new RepeatMatch
                {
                    BaseToken = "aab",
                    i = 0,
                    j = 5,
                    BaseGuesses = 1001,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 1000,
                            i = 0,
                            j = 2,
                            Token = "aab",
                        },
                    },
                    RepeatCount = 2,
                    Token = pattern,
                },
            };

            var result = matcher.MatchPassword(pattern);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesabRepeatInabababab()
        {
            var pattern = "abababab";
            var matcher = new RepeatMatcher();

            var expected = new[]
            {
                new RepeatMatch
                {
                    BaseToken = "ab",
                    i = 0,
                    j = 7,
                    BaseGuesses = 9,
                    BaseMatchItems = new List<Match>
                    {
                        new SequenceMatch
                        {
                            Guesses = 8,
                            i = 0,
                            j = 1,
                            Token = "ab",
                            Ascending = true,
                            SequenceName = "lower",
                            SequenceSpace = 26,
                        },
                    },
                    RepeatCount = 4,
                    Token = pattern,
                },
            };

            var result = matcher.MatchPassword(pattern);
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData("@", "y4@")]
        [InlineData("@", "u%7")]
        [InlineData("u", "y4@")]
        [InlineData("u", "u%7")]
        public void MatchesEmbeddedRepeatPatterns(string prefix, string suffix)
        {
            var pattern = "&&&&&";
            var password = $"{prefix}{pattern}{suffix}";
            var matcher = new RepeatMatcher();

            var expected = new[]
            {
                new RepeatMatch
                {
                    BaseToken = "&",
                    i = prefix.Length,
                    j = prefix.Length + pattern.Length - 1,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "&",
                        },
                    },
                    RepeatCount = 5,
                    Token = pattern,
                },
            };

            var result = matcher.MatchPassword(password);
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesMultipleAdjacentRepeats()
        {
            var matcher = new RepeatMatcher();

            var expected = new[]
            {
                new RepeatMatch
                {
                    BaseToken = "B",
                    i = 0,
                    j = 2,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "B",
                        },
                    },
                    RepeatCount = 3,
                    Token = "BBB",
                },
                new RepeatMatch
                {
                    BaseToken = "1",
                    i = 3,
                    j = 6,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "1",
                        },
                    },
                    RepeatCount = 4,
                    Token = "1111",
                },
                new RepeatMatch
                {
                    BaseToken = "a",
                    i = 7,
                    j = 11,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "a",
                        },
                    },
                    RepeatCount = 5,
                    Token = "aaaaa",
                },
                new RepeatMatch
                {
                    BaseToken = "@",
                    i = 12,
                    j = 17,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "@",
                        },
                    },
                    RepeatCount = 6,
                    Token = "@@@@@@",
                },
            };

            var result = matcher.MatchPassword("BBB1111aaaaa@@@@@@");
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MatchesMultipleRepeatsWithNonRepeatsInBetween()
        {
            var matcher = new RepeatMatcher();

            var expected = new[]
            {
                new RepeatMatch
                {
                    BaseToken = "B",
                    i = 4,
                    j = 6,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "B",
                        },
                    },
                    RepeatCount = 3,
                    Token = "BBB",
                },
                new RepeatMatch
                {
                    BaseToken = "1",
                    i = 12,
                    j = 15,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "1",
                        },
                    },
                    RepeatCount = 4,
                    Token = "1111",
                },
                new RepeatMatch
                {
                    BaseToken = "a",
                    i = 21,
                    j = 25,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "a",
                        },
                    },
                    RepeatCount = 5,
                    Token = "aaaaa",
                },
                new RepeatMatch
                {
                    BaseToken = "@",
                    i = 30,
                    j = 35,
                    BaseGuesses = 12,
                    BaseMatchItems = new List<Match>
                    {
                        new BruteForceMatch
                        {
                            Guesses = 11,
                            i = 0,
                            j = 0,
                            Token = "@",
                        },
                    },
                    RepeatCount = 6,
                    Token = "@@@@@@",
                },
            };

            var result = matcher.MatchPassword("2818BBBbzsdf1111@*&@!aaaaaEUDA@@@@@@1729");
            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('Z')]
        [InlineData('4')]
        [InlineData('&')]
        public void MatchesRepeatsWithBaseCharacters(char character)
        {
            var matcher = new RepeatMatcher();

            for (var i = 3; i <= 12; i++)
            {
                var pattern = new string(character, i);
                var expected = new[]
                {
                    new RepeatMatch
                    {
                        BaseToken = character.ToString(),
                        i = 0,
                        j = pattern.Length - 1,
                        BaseGuesses = 12,
                        BaseMatchItems = new List<Match>
                        {
                            new BruteForceMatch
                            {
                                Guesses = 11,
                                i = 0,
                                j = 0,
                                Token = character.ToString(),
                            },
                        },
                        RepeatCount = i,
                        Token = pattern,
                    },
                };

                var result = matcher.MatchPassword(pattern);
                result.Should().BeEquivalentTo(expected);
            }
        }

        [Fact]
        public void MatchNoRepeatedCharacters()
        {
            var repeat = new RepeatMatcher();

            var res = repeat.MatchPassword("asdf").ToList();

            res.Count.Should().Be(0);
        }

        [Fact]
        public void MatchRepeatedCharacters()
        {
            var repeat = new RepeatMatcher();

            var res = repeat.MatchPassword("aaasdffff").ToList();

            res.Count.Should().Be(2);

            res[0].i.Should().Be(0);
            res[0].j.Should().Be(2);
            res[0].Token.Should().Be("aaa");

            res[1].i.Should().Be(5);
            res[1].j.Should().Be(8);
            res[1].Token.Should().Be("ffff");
        }
    }
}
