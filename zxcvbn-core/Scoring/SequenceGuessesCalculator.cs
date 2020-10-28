using System.Collections.Generic;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    internal class SequenceGuessesCalculator
    {
        private static readonly List<char> obviousStartCharacters = new List<char>
        {
            'a', 'A', 'z', 'Z', '0', '1', '9'
        };

        public static double CalculateGuesses(SequenceMatch match)
        {
            int baseGuesses;

            if (obviousStartCharacters.Contains(match.Token[0]))
                baseGuesses = 4;
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
