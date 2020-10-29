using System;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to guess the date.
    /// </summary>
    internal class DateGuessesCalculator
    {
        /// <summary>
        /// The minimum distance between the reference date and the provided date.
        /// </summary>
        public const int MinimumYearSpace = 20;

        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static double CalculateGuesses(DateMatch match)
        {
            var yearSpace = Math.Max(Math.Abs(match.Year - DateMatcher.ReferenceYear), MinimumYearSpace);

            var guesses = yearSpace * 365.0;
            if (!string.IsNullOrEmpty(match.Separator))
                guesses *= 4;

            return guesses;
        }
    }
}
