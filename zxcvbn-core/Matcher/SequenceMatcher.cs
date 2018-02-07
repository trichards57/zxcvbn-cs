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

            void Update(int i, int j, int delta)
            {
                if (j - i <= 1 && Math.Abs(delta) != 1) return;
                if (0 >= Math.Abs(delta) || Math.Abs(delta) > MaxDelta) return;
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

                result.Add(new SequenceMatch
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
                Update(iIn, jIn, lastDelta);
                iIn = jIn;
                lastDelta = deltaIn;
            }

            Update(iIn, password.Length - 1, lastDelta);
            return result;
        }
    }
}