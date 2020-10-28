using System;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    internal class BruteForceGuessesCalculator
    {
        public const int MinSubmatchGuessesMultiCharacter = 50;
        public const int MinSubmatchGuessesSingleCharacter = 10;
        private const int BruteforceCardinality = 10;

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
