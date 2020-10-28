using System;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    internal class DateGuessesCalculator
    {
        public const int MinimumYearSpace = 20;

        public static double CalculateGuesses(DateMatch match)
        {
            var yearSpace = Math.Max(Math.Abs(match.Year - DateMatcher.ReferenceYear), MinimumYearSpace);

            var guesses = yearSpace * 365;
            if (!string.IsNullOrEmpty(match.Separator))
                guesses *= 4;

            return guesses;
        }
    }
}
