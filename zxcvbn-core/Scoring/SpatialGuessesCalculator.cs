using System;
using System.Linq;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    public class SpatialGuessesCalculator
    {
        private static readonly double KeyboardAverageDegree;
        private static readonly int KeyboardStartingPositions;
        private static readonly double KeypadAverageDegree;
        private static readonly int KeypadStartingPositions;

        static SpatialGuessesCalculator()
        {
            var matcher = new SpatialMatcher();
            KeyboardAverageDegree = CalculateAverageDegree(matcher.SpatialGraphs.First(s => s.Name == "qwerty"));
            KeyboardStartingPositions = matcher.SpatialGraphs.First(s => s.Name == "qwerty").AdjacencyGraph.Keys.Count;
            KeypadAverageDegree = CalculateAverageDegree(matcher.SpatialGraphs.First(s => s.Name == "keypad"));
            KeypadStartingPositions = matcher.SpatialGraphs.First(s => s.Name == "keypad").AdjacencyGraph.Keys.Count;
        }

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
                    guesses *= 2;
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
