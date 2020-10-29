using System;
using System.Globalization;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to guess the value picked out by regular expression.
    /// </summary>
    internal class RegexGuessesCalculator
    {
        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static double CalculateGuesses(RegexMatch match)
        {
            switch (match.RegexName)
            {
                case "recent_year":
                    var yearSpace = Math.Abs(int.Parse(match.Token, CultureInfo.InvariantCulture) - DateMatcher.ReferenceYear);
                    yearSpace = Math.Max(yearSpace, DateGuessesCalculator.MinimumYearSpace);
                    return yearSpace;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
