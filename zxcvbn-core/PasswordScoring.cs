using System;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn
{
    /// <summary>
    /// Some useful shared functions used for evaluating passwords
    /// </summary>
    internal static class PasswordScoring
    {
        public const string AllLower = "^[^A-Z]+$";
        public const string AllUpper = "^[^a-z]+$";
        public const string EndUpper = "^[^A-Z]+[A-Z]$";
        public const string StartUpper = "^[A-Z][^A-Z]+$";

        /// <summary>
        /// Caclulate binomial coefficient (i.e. nCk)
        /// Uses same algorithm as zxcvbn (cf. scoring.coffee), from http://blog.plover.com/math/choose.html
        /// </summary>
        /// <param name="k">k</param>
        /// <param name="n">n</param>
        /// <returns>Binomial coefficient; nCk</returns>
        public static long Binomial(int n, int k)
        {
            if (k > n) return 0;
            if (k == 0) return 1;

            long r = 1;
            for (var d = 1; d <= k; ++d)
            {
                r *= n;
                r /= d;
                n -= 1;
            }

            return r;
        }

        // ReSharper disable once InconsistentNaming
        public static long CaculateL33tVariations(DictionaryMatch match)
        {
            if (!match.L33t)
                return 1;

            if (!(match is L33tDictionaryMatch lMatch))
                return 1;

            var variations = 1L;

            foreach (var kvp in lMatch.Subs)
            {
                var unsubbed = kvp.Key;
                var subbed = kvp.Value;

                var chars = match.Token.ToLower().ToCharArray();

                var s = chars.Count(c => c == subbed);
                var u = chars.Count(c => c == unsubbed);

                if (s == 0 || u == 0)
                {
                    variations *= 2;
                }
                else
                {
                    var p = Math.Min(u, s);
                    var possibilities = 0L;
                    for (var i = 1; i < p; i++)
                    {
                        possibilities += Binomial(u + s, i);
                    }

                    variations *= possibilities;
                }
            }

            return variations;
        }

        /// <summary>
        /// Estimate the extra entropy in a token that comes from mixing upper and lowercase letters.
        /// This has been moved to a static function so that it can be used in multiple entropy calculations.
        /// </summary>
        /// <param name="word">The word to calculate uppercase entropy for</param>
        /// <returns>An estimation of the entropy gained from casing in <paramref name="word"/></returns>
        public static double CalculateUppercaseEntropy(string word)
        {
            if (Regex.IsMatch(word, AllLower)) return 0;

            // If the word is all uppercase add's only one bit of entropy, add only one bit for initial/end single cap only
            if (new[] { StartUpper, EndUpper, AllUpper }.Any(re => Regex.IsMatch(word, re))) return 1;

            var lowers = word.Count(c => 'a' <= c && c <= 'z');
            var uppers = word.Count(c => 'A' <= c && c <= 'Z');

            // Calculate numer of ways to capitalise (or inverse if there are fewer lowercase chars) and return lg for entropy
            return Math.Log(Enumerable.Range(0, Math.Min(uppers, lowers) + 1).Sum(i => Binomial(uppers + lowers, i)), 2);
        }

        public static long CalculateUppercaseVariations(DictionaryMatch match)
        {
            var word = match.Token;

            if (Regex.IsMatch(word, AllLower) || word.ToLower().Equals(word))
                return 1;

            // A capitalized word is the most common capitalization scheme, so it only doubles the search space (uncapitalzed + capitalized).
            // All caps and end-capitalized are common enough too, so underestimating as 2x factor to be safe.
            if (Regex.IsMatch(word, StartUpper) || Regex.IsMatch(word, EndUpper) || Regex.IsMatch(word, AllUpper))
                return 2;

            // Otherwise calculate the number of ways to capitalize U+L uppercase+lowercase letters with U uppercase letters or less.  If there's more uppercase than lower (e.g. PASSwORD) then use the number of ways to lowercase U+L letters with L lowercase letters or less.
            var u = word.ToCharArray().Count(c => Regex.IsMatch(c.ToString(), "[A-Z]"));
            var l = word.ToCharArray().Count(c => Regex.IsMatch(c.ToString(), "[a-z]"));

            var variations = 0L;

            for (var i = 1; i < Math.Min(u, l); i++)
            {
                variations += Binomial(u + l, i);
            }

            return variations;
        }

        /// <summary>
        /// Return a score for password strength from the crack time. Scores are 0..4, 0 being minimum and 4 maximum strength.
        /// </summary>
        /// <param name="crackTimeSeconds">Number of seconds estimated for password cracking</param>
        /// <returns>Password strength. 0 to 4, 0 is minimum</returns>
        public static int CrackTimeToScore(double crackTimeSeconds)
        {
            if (crackTimeSeconds < Math.Pow(10, 2)) return 0;
            if (crackTimeSeconds < Math.Pow(10, 4)) return 1;
            if (crackTimeSeconds < Math.Pow(10, 6)) return 2;
            if (crackTimeSeconds < Math.Pow(10, 8)) return 3;
            return 4;
        }

        /// <summary>
        /// Calculate a rough estimate of crack time for entropy, see zxcbn scoring.coffee for more information on the model used
        /// </summary>
        /// <param name="entropy">Entropy of password</param>
        /// <returns>An estimation of seconts taken to crack password</returns>
        public static double EntropyToCrackTime(double entropy)
        {
            const double singleGuess = 0.01;
            const double numAttackers = 100;
            const double secondsPerGuess = singleGuess / numAttackers;

            return 0.5 * Math.Pow(2, entropy) * secondsPerGuess;
        }

        public static SequenceMatch MostGuessableMatchSequence(string baseToken, Result matchPassword)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculate the cardinality of the minimal character sets necessary to brute force the password (roughly)
        /// (e.g. lowercase = 26, numbers = 10, lowercase + numbers = 36)
        /// </summary>
        /// <param name="password">THe password to evaluate</param>
        /// <returns>An estimation of the cardinality of charactes for this password</returns>
        public static int PasswordCardinality(string password)
        {
            var cl = 0;

            if (password.Any(c => 'a' <= c && c <= 'z')) cl += 26; // Lowercase
            if (password.Any(c => 'A' <= c && c <= 'Z')) cl += 26; // Uppercase
            if (password.Any(c => '0' <= c && c <= '9')) cl += 10; // Numbers
            if (password.Any(c => c <= '/' || (':' <= c && c <= '@') || ('[' <= c && c <= '`') || ('{' <= c && c <= 0x7F))) cl += 33; // Symbols
            if (password.Any(c => c > 0x7F)) cl += 100; // 'Unicode' (why 100?)

            return cl;
        }
    }
}