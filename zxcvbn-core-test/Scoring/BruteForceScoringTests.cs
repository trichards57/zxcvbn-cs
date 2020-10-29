using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Tests.Scoring
{
    public class BruteForceScoringTests
    {
        [Fact]
        public void MostGuessableMatchSequenceChoosesMatchWithFewestGuessesIfTheyMatchTheSameSpan()
        {
            var password = "0123456789";
            var worseMatch = CreateTestMatch(0, 9, 1);
            var bestMatch = CreateTestMatch(0, 9, 2);

            var expected = new List<Match>
            {
                 worseMatch,
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { worseMatch, bestMatch }, true);
            result.Sequence.Should().BeEquivalentTo(expected);

            result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { bestMatch, worseMatch }, true);
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MostGuessableMatchSequenceChoosesOptimalMatches()
        {
            var password = "0123456789";
            var m0 = CreateTestMatch(0, 9, 3);
            var m1 = CreateTestMatch(0, 3, 2);
            var m2 = CreateTestMatch(4, 9, 1);

            var expected = new List<Match>
            {
                 m0,
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { m0, m1, m2 }, true);
            result.Sequence.Should().BeEquivalentTo(expected);

            m0.Guesses = 5;

            expected = new List<Match>
            {
                 m1, m2,
            };

            result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { m0, m1, m2 }, true);
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MostGuessableMatchSequenceReturnsBruteForceThenMatchThenBruteForceWhenMatchCoversAnInfixOfPassword()
        {
            var password = "0123456789";
            var existingMatch = CreateTestMatch(1, 8, 1);

            var expected = new List<Match>
            {
                new BruteForceMatch
                {
                    i = 0,
                    j = 0,
                    Token = "0",
                    Guesses = 11,
                },
                existingMatch,
                new BruteForceMatch
                {
                    i = 9,
                    j = 9,
                    Token = "9",
                    Guesses = 11,
                },
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { existingMatch }, true);
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MostGuessableMatchSequenceReturnsBruteForceThenMatchWhenMatchCoversASuffixOfPassword()
        {
            var password = "0123456789";
            var existingMatch = CreateTestMatch(3, 9, 1);

            var expected = new List<Match>
            {
                 new BruteForceMatch
                 {
                    i = 0,
                    j = 2,
                    Token = "012",
                    Guesses = 1000,
                 },
                 existingMatch,
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { existingMatch }, true);
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MostGuessableMatchSequenceReturnsMatchThenBruteForceWhenMatchCoversAPrefixOfPassword()
        {
            var password = "0123456789";
            var existingMatch = CreateTestMatch(0, 5, 1);

            var expected = new List<Match>
            {
                existingMatch,
                new BruteForceMatch
                {
                    i = 6,
                    j = 9,
                    Token = "6789",
                    Guesses = 10000,
                },
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, new List<Match> { existingMatch }, true);
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void MostGuessableMatchSequenceRetursnOneMatchForEmptySequence()
        {
            var password = "0123456789";
            var expected = new List<Match>
            {
                new BruteForceMatch
                {
                    i = 0,
                    j = 9,
                    Token = password,
                    Guesses = 10000000000,
                },
            };

            var result = PasswordScoring.MostGuessableMatchSequence(password, Enumerable.Empty<Match>());
            result.Sequence.Should().BeEquivalentTo(expected);
        }

        private Match CreateTestMatch(int i, int j, double guesses)
        {
            return new DateMatch
            {
                i = i,
                j = j,
                Guesses = guesses,
                Token = "abc",
                Day = 1,
                Month = 2,
                Separator = "/",
                Year = 3,
            };
        }
    }
}
