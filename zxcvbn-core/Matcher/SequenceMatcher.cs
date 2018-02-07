using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Zxcvbn.Matcher.Matches;

namespace Zxcvbn.Matcher
{
    /// <inheritdoc />
    /// <summary>
    /// This matcher detects lexicographical sequences (and in reverse) e.g. abcd, 4567, PONML etc.
    /// </summary>
    public class SequenceMatcher : IMatcher
    {
        private const int MaxDelta = 5;
        private const string SequencePattern = "sequence";

        private readonly string[] _sequenceNames = {
            "lower",
            "upper",
            "digits"
        };

        // Sequences should not overlap, sequences here must be ascending, their reverses will be checked automatically
        private readonly string[] _sequences = {
            "abcdefghijklmnopqrstuvwxyz",
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "01234567890"
        };

        /// <inheritdoc />
        /// <summary>
        /// Find matching sequences in <paramref name="password" />
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>Enumerable of sqeunec matches</returns>
        /// <seealso cref="T:Zxcvbn.Matcher.SequenceMatch" />
        public IEnumerable<Match> MatchPassword(string password)
        {
            if (password.Length == 1)
                return Enumerable.Empty<Match>();

            var result = new List<Match>();

            void update(int i, int j, int delta)
            {
                if (j - i > 1 || Math.Abs(delta) == 1)
                {
                    if (0 < Math.Abs(delta) && Math.Abs(delta) <= MaxDelta)
                    {
                        var token = password.Substring(i, j - i);
                        string sequenceName;
                        int sequenceSpace;

                        if (Regex.IsMatch(token, "^[a-z]+$"))
                        {
                            sequenceName = "lower";
                            sequenceSpace = 26;
                        }
                        else if (Regex.IsMatch(token, "^[A-Z]+$"))
                        {
                            sequenceName = "upper";
                            sequenceSpace = 26;
                        }
                        else if (Regex.IsMatch(token, "^\\d+$"))
                        {
                            sequenceName = "digits";
                            sequenceSpace = 10;
                        }
                        else
                        {
                            sequenceName = "unicode";
                            sequenceSpace = 26;
                        }

                        result.Add(new SequenceMatch()
                        {
                            Pattern = SequencePattern,
                            i = i,
                            j = j,
                            Token = token,
                            SequenceName = sequenceName,
                            SequenceSpace = sequenceSpace,
                            Ascending = delta > 0
                        });
                    }
                }
            }

            var iIn = 0;
            var lastDelta = -1;

            for (var k = 1; k < password.Length; k++)
            {
                var deltaIn = password[k] - password[k - 1];
                if (lastDelta < 0)
                    lastDelta = deltaIn;
                if (deltaIn == lastDelta)
                    continue;

                var jIn = k - 1;
                update(iIn, jIn, lastDelta);
                iIn = jIn;
                lastDelta = deltaIn;
            }

            update(iIn, password.Length - 1, lastDelta);
            return result;
        }

        private static double CalculateEntropy(string match, bool ascending)
        {
            var firstChar = match[0];

            // XXX: This entropy calculation is hard coded, ideally this would (somehow) be derived from the sequences above
            double baseEntropy;
            if (firstChar == 'a' || firstChar == '1') baseEntropy = 1;
            else if ('0' <= firstChar && firstChar <= '9') baseEntropy = Math.Log(10, 2); // Numbers
            else if ('a' <= firstChar && firstChar <= 'z') baseEntropy = Math.Log(26, 2); // Lowercase
            else baseEntropy = Math.Log(26, 1) + 1; // + 1 for uppercase

            if (!ascending) baseEntropy += 1; // Descending instead of ascending give + 1 bit of entropy

            return baseEntropy + Math.Log(match.Length, 2);
        }
    }
}