using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to guess the sequence.
    /// </summary>
    internal class SequenceGuessesCalculator
    {
        private static readonly List<char> ObviousStartCharacters = new List<char>
        {
            'a', 'A', 'z', 'Z', '0', '1', '9',
        };

        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static long CalculateGuesses(SequenceMatch match)
        {
            int baseGuesses;

            if (ObviousStartCharacters.Contains(match.Token[0]))
            {
                baseGuesses = 4;
            }
            else
            {
                if (char.IsDigit(match.Token[0]))
                    baseGuesses = 10;
                else
                    baseGuesses = 26;
            }

            if (!match.Ascending)
                baseGuesses *= 2;

            return baseGuesses * match.Token.Length;
        }
    }
}
