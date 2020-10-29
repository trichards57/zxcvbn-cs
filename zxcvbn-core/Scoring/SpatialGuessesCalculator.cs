using System;
using System.Linq;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to guess the spatial pattern.
    /// </summary>
    internal static class SpatialGuessesCalculator
    {
        /// <summary>
        /// The average number of adjacent characters on a keyboard.
        /// </summary>
        internal static readonly double KeyboardAverageDegree = (int)Math.Round(CalculateAverageDegree(SpatialMatcher.SpatialGraphs.First(s => s.Name == "qwerty")));

        /// <summary>
        /// The number of starting positions on a keyboard.
        /// </summary>
        internal static readonly int KeyboardStartingPositions = SpatialMatcher.SpatialGraphs.First(s => s.Name == "qwerty").AdjacencyGraph.Keys.Count;

        /// <summary>
        /// The average number of adjacent characters on a keypad.
        /// </summary>
        internal static readonly double KeypadAverageDegree = (int)Math.Round(CalculateAverageDegree(SpatialMatcher.SpatialGraphs.First(s => s.Name == "keypad")));

        /// <summary>
        /// The number of starting positions on a keypad.
        /// </summary>
        internal static readonly int KeypadStartingPositions = SpatialMatcher.SpatialGraphs.First(s => s.Name == "keypad").AdjacencyGraph.Keys.Count;

        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static double CalculateGuesses(SpatialMatch match)
        {
            int s;
            double d;
            if (match.Graph == "qwerty" || match.Graph == "dvorak")
            {
                s = KeyboardStartingPositions;
                d = KeyboardAverageDegree;
            }
            else
            {
                s = KeypadStartingPositions;
                d = KeypadAverageDegree;
            }

            double guesses = 0;
            var l = match.Token.Length;
            var t = match.Turns;

            for (var i = 2; i <= l; i++)
            {
                var possibleTurns = Math.Min(t, i - 1);
                for (var j = 1; j <= possibleTurns; j++)
                {
                    guesses += PasswordScoring.Binomial(i - 1, j - 1) * s * Math.Pow(d, j);
                }
            }

            if (match.ShiftedCount > 0)
            {
                var shifted = match.ShiftedCount;
                var unshifted = match.Token.Length - match.ShiftedCount;
                if (shifted == 0 || unshifted == 0)
                {
                    guesses *= 2;
                }
                else
                {
                    double variations = 0;
                    for (var i = 1; i <= Math.Min(shifted, unshifted); i++)
                    {
                        variations += PasswordScoring.Binomial(shifted + unshifted, i);
                    }

                    guesses *= variations;
                }
            }

            return guesses;
        }

        private static double CalculateAverageDegree(SpatialGraph graph)
        {
            var average = 0.0;
            foreach (var key in graph.AdjacencyGraph.Keys)
            {
                average += graph.AdjacencyGraph[key].Count(s => s != null);
            }

            average /= graph.AdjacencyGraph.Keys.Count;
            return average;
        }
    }
}
