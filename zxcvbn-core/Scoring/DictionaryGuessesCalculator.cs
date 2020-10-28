using System;
using System.Globalization;
using System.Linq;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Scoring
{
    /// <summary>
    /// Estimates the number of attempts needed to dictionary search for the password.
    /// </summary>
    internal class DictionaryGuessesCalculator
    {
        /// <summary>
        /// Estimates the attempts required to guess the password.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The guesses estimate.</returns>
        public static long CalculateGuesses(DictionaryMatch match)
        {
            match.BaseGuesses = match.Rank;
            match.UppercaseVariations = UppercaseVariations(match.Token);
            match.L33tVariations = L33tVariations(match);
            var reversedVariations = match.Reversed ? 2 : 1;

            return match.BaseGuesses * match.UppercaseVariations * match.L33tVariations * reversedVariations;
        }

        /// <summary>
        /// Calculates the number of l33t variations in the word.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns>The number of possible variations.</returns>
        internal static long L33tVariations(DictionaryMatch match)
        {
            if (!match.L33t)
                return 1;

            long variations = 1;

            foreach (var subbed in match.Sub.Keys)
            {
                var unsubbed = match.Sub[subbed];
                var s = match.Token.ToLower(CultureInfo.InvariantCulture).Count(c => c == subbed);
                var u = match.Token.ToLower(CultureInfo.InvariantCulture).Count(c => c == unsubbed);

                if (s == 0 || u == 0)
                {
                    variations *= 2;
                }
                else
                {
                    var p = Math.Min(u, s);
                    long possibilities = 0;
                    for (var i = 1; i <= p; i++)
                        possibilities += PasswordScoring.Binomial(u + s, i);
                    variations *= possibilities;
                }
            }

            return variations;
        }

        /// <summary>
        /// Calculates the number of uppercase variations in the word.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The number of possible variations.</returns>
        internal static long UppercaseVariations(string token)
        {
            if (token.All(c => char.IsLower(c)) || token.ToLower(CultureInfo.InvariantCulture) == token)
                return 1;

            if ((char.IsUpper(token.First()) && token.Skip(1).All(c => char.IsLower(c)))
                || token.All(c => char.IsUpper(c))
                || (char.IsUpper(token.Last()) && token.Take(token.Length - 1).All(c => char.IsLower(c))))
                return 2;

            var u = token.Count(c => char.IsUpper(c));
            var l = token.Count(c => char.IsLower(c));
            long variations = 0;

            for (var i = 1; i <= Math.Min(u, l); i++)
                variations += PasswordScoring.Binomial(u + l, i);

            return variations;
        }
    }
}
