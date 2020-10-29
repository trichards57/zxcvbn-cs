using System;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to brute-force the password.
    /// </summary>
    internal class BruteForceGuessesCalculator
    {
        /// <summary>
        /// The minimum submatch guesses for a multi character string.
        /// </summary>
        public const double MinSubmatchGuessesMultiCharacter = 50;

        /// <summary>
        /// The minimum submatch guesses for a single character.
        /// </summary>
        public const double MinSubmatchGuessesSingleCharacter = 10;

        private const double BruteforceCardinality = 10;

        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static double CalculateGuesses(BruteForceMatch match)
        {
            var guesses = Math.Pow(BruteforceCardinality, match.Token.Length);
            if (double.IsPositiveInfinity(guesses))
                guesses = double.MaxValue;

            var minGuesses = match.Token.Length == 1 ? MinSubmatchGuessesSingleCharacter + 1 : MinSubmatchGuessesMultiCharacter + 1;

            return Math.Max(guesses, minGuesses);
        }
    }
}
