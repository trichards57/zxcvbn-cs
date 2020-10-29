using FluentAssertions;
using System;
using Xunit;
using Zxcvbn.Matcher.Matches;
using Zxcvbn.Scoring;

namespace Zxcvbn.Tests.Scoring
{
    public class SpatialTests
    {
        [Fact]
        public void AccountsForTurnPositionsDirectionsAndStartingKey()
        {
            var match = new SpatialMatch
            {
                Token = "zxcvbn",
                Graph = "qwerty",
                Turns = 3,
                ShiftedCount = 0,
                i = 1,
                j = 2,
            };

            var l = match.Token.Length;
            var s = SpatialGuessesCalculator.KeyboardStartingPositions;
            var d = SpatialGuessesCalculator.KeyboardAverageDegree;
            var expected = 0.0;

            for (var i = 2; i <= l; i++)
            {
                for (var j = 1; j <= Math.Min(match.Turns, i - 1); j++)
                {
                    expected += PasswordScoring.Binomial(i - 1, j - 1) * s * Math.Pow(d, j);
                }
            }

            var actual = SpatialGuessesCalculator.CalculateGuesses(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void AddsToTheGuessesForShiftedKeys()
        {
            var match = new SpatialMatch
            {
                Token = "zxcvbn",
                Graph = "qwerty",
                Turns = 1,
                ShiftedCount = 2,
                i = 1,
                j = 2,
            };

            var expected = SpatialGuessesCalculator.KeyboardStartingPositions * SpatialGuessesCalculator.KeyboardAverageDegree * (match.Token.Length - 1) * 21;

            var actual = SpatialGuessesCalculator.CalculateGuesses(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void DoublesGuessesIfEverythingIsShifted()
        {
            var match = new SpatialMatch
            {
                Token = "zxcvbn",
                Graph = "qwerty",
                Turns = 1,
                ShiftedCount = 6,
                i = 1,
                j = 2,
            };

            var expected = SpatialGuessesCalculator.KeyboardStartingPositions * SpatialGuessesCalculator.KeyboardAverageDegree * (match.Token.Length - 1) * 2;

            var actual = SpatialGuessesCalculator.CalculateGuesses(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GuessesWhenThereAreNoTurns()
        {
            var match = new SpatialMatch
            {
                Token = "zxcvbn",
                Graph = "qwerty",
                Turns = 1,
                ShiftedCount = 0,
                i = 1,
                j = 2,
            };

            var expected = SpatialGuessesCalculator.KeyboardStartingPositions * SpatialGuessesCalculator.KeyboardAverageDegree * (match.Token.Length - 1);

            var actual = SpatialGuessesCalculator.CalculateGuesses(match);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsDelegatedToByEstimateGuesses()
        {
            var match = new SpatialMatch
            {
                Token = "zxcvbn",
                Graph = "qwerty",
                Turns = 1,
                ShiftedCount = 0,
                i = 1,
                j = 2,
            };

            var expected = SpatialGuessesCalculator.KeyboardStartingPositions * SpatialGuessesCalculator.KeyboardAverageDegree * (match.Token.Length - 1);

            var actual = PasswordScoring.EstimateGuesses(match, match.Token);

            actual.Should().Be(expected);
        }
    }
}
